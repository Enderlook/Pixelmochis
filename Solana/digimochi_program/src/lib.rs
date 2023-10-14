use mpl_token_metadata::{accounts::Metadata, ID as metadata_program};
use solana_program::{
    account_info::{next_account_info, AccountInfo},
    clock::Clock,
    entrypoint,
    entrypoint::ProgramResult,
    native_token::LAMPORTS_PER_SOL,
    program::{invoke, invoke_signed},
    program_error::ProgramError,
    program_pack::Pack,
    pubkey::Pubkey,
    rent::Rent,
    system_instruction::{create_account, transfer},
    system_program,
    sysvar::Sysvar,
};
use spl_token::state::{Account, Mint};

// Note: We don't use Borsh for serializing for some reasons:
// 1) Our data is trivial (3 integers), so I can parse it manually.
// 2) We want to deserialize in Unity, and the Borsh library for .NET doesn't support .NET Standard 2.1.
//   (We could use Borsh for Javascript but that would complicate a lot more our workflow).

// Encoding of each instruction.
const FEED: u8 = 1;
const CURE: u8 = 2;
const BATH: u8 = 3;
const SEED: &[u8] = b"vault";
const METADATA_SEED: &[u8] = b"metadata";

// Space consumed in the blockchain.
// Numerical values are stored in little endian.
// Offset   Size    Description
//      0      1    Version, useful is we make updates in the serialization. Current version is `1`.
//      1      8    Last meal, as `i64` seconds since Unix Epoch.
//      9      8    Last medicine, as `i64` seconds since Unix Epoch.
//     17      8    Last bath, as `i64` seconds since Unix Epoch.
//     25      1    Level, as `u8`, starting from `1`. **RESERVED**
//     26      8    Experience. **RESERVED**
const SPACE: usize = 34;
const SPACE_: u64 = 34;

const CREATOR: Pubkey = Pubkey::new_from_array([
    123, 134, 1, 77, 240, 10, 178, 223, 4, 42, 17, 185, 195, 45, 9, 120, 35, 214, 166, 124, 187,
    49, 53, 224, 238, 250, 187, 45, 108, 82, 33,
    32,
]);

/*const ROYALTY: Pubkey = Pubkey::new_from_array([]);

const ROYALTY_LAMPORTS: u64 = LAMPORTS_PER_SOL / 100;*/

entrypoint!(process_instruction);

pub fn process_instruction(
    program_id: &Pubkey,
    accounts: &[AccountInfo],
    instruction_data: &[u8],
) -> ProgramResult {
    let accounts_iter = &mut accounts.iter();
    let payer_account = next_account_info(accounts_iter)?;
    let pet_token_account = next_account_info(accounts_iter)?;
    let pet_mint_account = next_account_info(accounts_iter)?;
    let pet_data_account = next_account_info(accounts_iter)?;
    let metadata_account = next_account_info(accounts_iter)?;
    //let royalty_account = next_account_info(accounts_iter)?;
    let system_program = next_account_info(accounts_iter)?;

    // Check if system program is valid.
    if !system_program::check_id(system_program.key) {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }

    // Check if accounts has required sign and writing permissions.
    if !payer_account.is_signer || !payer_account.is_writable || !pet_data_account.is_writable
    /*|| !royalty_account.is_writable*/
    {
        return ProgramResult::Err(ProgramError::MissingRequiredSignature);
    }

    // Check if instructions are correct.
    let instruction_data: Result<[u8; 3], _> = instruction_data.try_into();
    let instruction_data = match instruction_data {
        Ok([a, b, FEED]) => [a, b, FEED],
        Ok([a, b, CURE]) => [a, b, CURE],
        Ok([a, b, BATH]) => [a, b, BATH],
        Err(_) | Ok(_) => return ProgramResult::Err(ProgramError::InvalidInstructionData),
    };

    /*// Check if royalty account is correct.
    if royalty_account.key != &ROYALTY {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }*/

    // Check if the metadata provided is the expected.
    let bump_seed = instruction_data[1];
    let seeds = [
        METADATA_SEED,
        metadata_program.as_ref(),
        pet_mint_account.key.as_ref(),
        &[bump_seed],
    ];
    let actual_metadata_account_key = Pubkey::create_program_address(&seeds, &metadata_program)?;
    if &actual_metadata_account_key != metadata_account.key {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }

    // Check NFT was created by us.
    let metadata_data = Metadata::from_bytes(
        *metadata_account
            .data
            .try_borrow()
            .map_err(|_| ProgramError::AccountBorrowFailed)?,
    )?;
    let mut has_creator = false;
    for creator in metadata_data
        .creators
        .map_or(Err(ProgramError::InvalidAccountData), Ok)?
    {
        if creator.verified && creator.address == CREATOR {
            has_creator = true;
            break;
        }
    }
    if !has_creator {
        return ProgramResult::Err(ProgramError::InvalidAccountData);
    }

    let pet_mint_data = Mint::unpack(
        *pet_mint_account
            .data
            .try_borrow()
            .map_err(|_| ProgramError::AccountBorrowFailed)?,
    )?;
    // Check if mint is from a single NFT.
    // TODO: Do we actually need to check this taking into account that we are creating the NFTs?
    if pet_mint_data.supply != 1 || pet_mint_data.decimals != 0 {
        return ProgramResult::Err(ProgramError::InvalidAccountData);
    }

    let pet_token_data = Account::unpack(
        *pet_token_account
            .data
            .try_borrow()
            .map_err(|_| ProgramError::AccountBorrowFailed)?,
    )?;
    // Check if pet token is from pet mint.
    if &pet_token_data.mint != pet_mint_account.key {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }

    // Check if the payer is the owner of the pet token.
    if &pet_token_data.owner != payer_account.key {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }

    // Check if user actually has the NFT.
    if pet_token_data.amount != 1 {
        return ProgramResult::Err(ProgramError::InvalidAccountData);
    }

    // Check if the pet data provided is the expected.
    let bump_seed = instruction_data[0];
    let seeds = [SEED, pet_token_data.mint.as_ref(), &[bump_seed]];
    let actual_pet_data_key = Pubkey::create_program_address(&seeds, program_id)?;
    if &actual_pet_data_key != pet_data_account.key {
        return ProgramResult::Err(ProgramError::InvalidArgument);
    }

    let mut pet_data = pet_data_account
        .data
        .try_borrow_mut()
        .map_err(|_| ProgramError::AccountBorrowFailed)?;

    // Get time.
    // We get it here rather than after the last validation so we don't write it twice.
    // It makes the program smaller.
    let time_bytes = &Clock::get()?.unix_timestamp.to_le_bytes();

    // Check if the account does exists.
    // If an account doesn't exists, its owner is not us but system program.
    if pet_data_account.owner != program_id {
        if pet_data_account.owner != system_program.key {
            return ProgramResult::Err(ProgramError::InvalidArgument);
        }
        drop(pet_data);
        let lamports_required = Rent::get()?.minimum_balance(SPACE);
        invoke_signed(
            &create_account(
                payer_account.key,
                pet_data_account.key,
                lamports_required,
                SPACE_,
                program_id,
            ),
            &[
                payer_account.clone(),
                pet_data_account.clone(),
                system_program.clone(),
            ],
            &[&seeds],
        )?;
        pet_data = pet_data_account
            .data
            .try_borrow_mut()
            .map_err(|_| ProgramError::AccountBorrowFailed)?;

        // Stores versioning of data.
        match pet_data.get_mut(0) {
            Some(data) => *data = 1u8,
            None => return ProgramResult::Err(ProgramError::InvalidAccountData),
        }

        // Set level.
        match pet_data.get_mut(25) {
            Some(data) => *data = 1u8,
            None => return ProgramResult::Err(ProgramError::InvalidAccountData),
        }
    }

    /*invoke(
        &transfer(payer_account.key, &ROYALTY, ROYALTY_LAMPORTS),
        &[payer_account.clone(), royalty_account.clone()],
    )?;*/

    match match instruction_data[2] {
        FEED => pet_data.get_mut(1..9),
        CURE => pet_data.get_mut(10..18),
        BATH => pet_data.get_mut(19..27),
        // Safety, we checked this above at the beginning of the program.
        _ => unsafe { core::hint::unreachable_unchecked() },
    } {
        Some(data) => data.copy_from_slice(time_bytes),
        _ => return ProgramResult::Err(ProgramError::InvalidAccountData),
    }

    Ok(())
}
