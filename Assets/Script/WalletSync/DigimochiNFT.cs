using Solana.Unity.Metaplex.NFT.Library;
using Solana.Unity.Metaplex.Utilities.Json;
using Solana.Unity.Programs;
using Solana.Unity.Rpc.Core.Http;
using Solana.Unity.Rpc.Messages;
using Solana.Unity.Rpc.Models;
using Solana.Unity.Rpc.Types;
using Solana.Unity.SDK;
using Solana.Unity.SDK.Nft;
using Solana.Unity.Wallet;

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using Attribute = Solana.Unity.Metaplex.Utilities.Json.Attribute;
using Creator = Solana.Unity.Metaplex.NFT.Library.Creator;

namespace Pixelmotchis
{
    public sealed class DigimochiNFT : IDigimochiData
    {
        private static readonly byte[] VaultSeed = Encoding.UTF8.GetBytes("vault");
        private static readonly byte[] MetadataSeed = Encoding.UTF8.GetBytes("metadata");
        private static readonly PublicKey TokenMetadataProgramPublicKey = new("metaqbxxUerdq28cj1RbAWkYQm3ybzjb6a8bt518x1s");
        private static readonly PublicKey ProgramPublicKey = new("8GdnC5JNZkJgAvfPV7qMvRxjv2EACpqePSaqz89XiJPh");
        private static readonly PublicKey CreatorPublicKey = new("9KBfNf8UYwnRjq1fFRUb1Mn6iqGd239X3PG2Vvwmg7Zq");

        private readonly Nft nft;
        private readonly byte digimochiBumpSeed;
        private readonly byte metadataBumpSeed;
        private readonly AccountMeta[] keys;
        private readonly string species;
        private readonly PublicKey digimochiDataAddress;

        private DateTime lastMeal;
        private DateTime lastMedicine;
        private DateTime lastBath;

        private DigimochiNFT(Nft nft, AccountMeta[] keys, PublicKey digimochiDataAddress, byte digimochiBumpSeed, byte metadataBumpSeed, string species)
        {
            this.nft = nft;
            this.keys = keys;
            this.digimochiDataAddress = digimochiDataAddress;
            this.digimochiBumpSeed = digimochiBumpSeed;
            this.metadataBumpSeed = metadataBumpSeed;
            this.species = species;
        }

        public string GetName() => nft.metaplexData.data.metadata.name;

        public string GetDigimochiType() => species;

        public DateTime GetLastMealTime() => lastMeal;

        public DateTime GetLastMedicineTime() => lastMedicine;

        public DateTime GetLastBathTime() => lastBath;

        public async Task<bool> Feed() => await ExecuteProgram(Action.Feed);

        public async Task<bool> Cure() => await ExecuteProgram(Action.Cure);

        public async Task<bool> Bath() => await ExecuteProgram(Action.Bath);

        public async Task UpdateDataAccount()
        {
            RequestResult<ResponseValue<AccountInfo>> petDataAccount = await Web3.Wallet.ActiveRpcClient.GetAccountInfoAsync(digimochiDataAddress);
            if (petDataAccount.WasSuccessful && petDataAccount.Result.Value?.Data is List<string> data && data.Count == 2)
            {
                Debug.Assert(data[1] == "base64");
                byte[] payload = Convert.FromBase64String(data[0]);
                if ((payload?.Length ?? 0) > 0)
                {
                    long secondsSinceEpoch = BinaryPrimitives.ReadInt64LittleEndian(payload.AsSpan(1, 8));
                    if (secondsSinceEpoch != 0)
                        lastMeal = DateTime.UnixEpoch.AddSeconds(secondsSinceEpoch);

                    secondsSinceEpoch = BinaryPrimitives.ReadInt64LittleEndian(payload.AsSpan(9, 8));
                    if (secondsSinceEpoch != 0)
                        lastMedicine = DateTime.UnixEpoch.AddSeconds(secondsSinceEpoch);

                    secondsSinceEpoch = BinaryPrimitives.ReadInt64LittleEndian(payload.AsSpan(17, 8));
                    if (secondsSinceEpoch != 0)
                        lastBath = DateTime.UnixEpoch.AddSeconds(secondsSinceEpoch);
                }
            }
        }

        private enum Action : byte
        {
            Feed = 1,
            Cure = 2,
            Bath = 3,
        }

        private async Task<bool> ExecuteProgram(Action action)
        {
            Transaction transaction = new();
            transaction.Add(new TransactionInstruction()
            {
                ProgramId = ProgramPublicKey.KeyBytes,
                Keys = keys,
                Data = new byte[] { digimochiBumpSeed, metadataBumpSeed, (byte)action },
            });
            transaction.FeePayer = Web3.Account.PublicKey;

        again:
            RequestResult<ResponseValue<LatestBlockHash>> recentBlockHashRequest = await Web3.Wallet.ActiveRpcClient.GetLatestBlockHashAsync();
            if (!recentBlockHashRequest.WasSuccessful)
            {
                // Errors which could be product of timeouts or things which require retry mechanism.
                if (recentBlockHashRequest.ServerErrorCode is -32002 or -32004 or -32005 or -32014)
                {
                    goto again;
                }
                else
                {
                    Debug.LogError($"Error {recentBlockHashRequest.RawRpcResponse}");
                    return false;
                }
            }
            else
            {
                transaction.RecentBlockHash = recentBlockHashRequest.Result.Value.Blockhash;
            }

            Transaction signedTransaction;
            try
            {
                signedTransaction = await Web3.Wallet.SignTransaction(transaction);
            }
            catch
            {
                return false;
            }
            RequestResult<string> transactionRequest = await Web3.Wallet.ActiveRpcClient.SendAndConfirmTransactionAsync(signedTransaction.Serialize());

            if (!transactionRequest.WasSuccessful)
            {
                // Errors which could be product of timeouts or things which require retry mechanism.
                if (transactionRequest.ServerErrorCode is -32002 or -32004 or -32005 or -32014)
                {
                    goto again;
                }

                Debug.LogError($"Error {transactionRequest.RawRpcResponse}");
                return false;
            }

            await UpdateDataAccount();

            return true;
        }

        public static async IAsyncEnumerable<DigimochiNFT> GetAllDigimochis()
        {
            TokenAccount[] tokens = await Web3.Wallet.GetTokenAccounts(Commitment.Confirmed);
            foreach (TokenAccount token in tokens)
            {
                TokenAccountInfoDetails info = token.Account.Data.Parsed.Info;
                if (info.TokenAmount.AmountDecimal != 1)
                    continue;

                PublicKey mintAddress = new(info.Mint);

                if (!PublicKey.TryFindProgramAddress(
                    new byte[][] { VaultSeed, mintAddress.KeyBytes },
                    ProgramPublicKey,
                    out PublicKey digimochiDataAddress,
                    out byte digimochiDataBumpSeed))
                    continue;

                Nft nft = await Nft.TryGetNftData(mintAddress, Web3.Instance.WalletBase.ActiveRpcClient, loadTexture: false);

                if (nft?.metaplexData?.data is not MetadataAccount metadataAccount)
                    continue;

                if (metadataAccount.offchainData is not MetaplexTokenStandard metaplexTokenStandard)
                    continue;

                if (metadataAccount.metadata?.creators is not IList<Creator> creators)
                    continue;

                foreach (Creator creator in creators)
                {
                    if (creator.verified && creator.key == CreatorPublicKey)
                        goto foundCreator;
                }
                continue;
            foundCreator:

                if (metaplexTokenStandard.attributes is not List<Attribute> attributes)
                    continue;

                // There seems to be a bug in the Solana NFT parser because `trait_type` is always null.
                string species = attributes[0].value;
                /*string species;
                foreach (Attribute attribute in attributes)
                {
                    {
                        if (attribute.trait_type == "Species")
                        {
                            species = attribute.value;
                            goto foundSpecies;
                        }
                    }
                }
                continue;
            foundSpecies:*/

                if (!PublicKey.TryFindProgramAddress(
                    new byte[][] { MetadataSeed, MetadataProgram.ProgramIdKey, mintAddress.KeyBytes },
                    TokenMetadataProgramPublicKey,
                    out PublicKey metadataAddress,
                    out byte metadataBumpSeed))
                    continue;

                AccountMeta[] accountMeta = new AccountMeta[]
                {
                    AccountMeta.Writable(Web3.Account.PublicKey, true),
                    AccountMeta.ReadOnly(new(token.PublicKey), false),
                    AccountMeta.ReadOnly(mintAddress, false),
                    AccountMeta.Writable(digimochiDataAddress, false),
                    AccountMeta.ReadOnly(metadataAddress, false),
                    AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                };

                DigimochiNFT digimochi = new(nft, accountMeta, digimochiDataAddress, digimochiDataBumpSeed, metadataBumpSeed, species);

                await digimochi.UpdateDataAccount();

                if (digimochi.lastMeal == DateTime.UnixEpoch || digimochi.lastMedicine == DateTime.UnixEpoch || digimochi.lastBath == DateTime.UnixEpoch)
                {
                    string beforeSignature = null;
                    DateTime last = DateTime.UnixEpoch;
                    while (true)
                    {
                        RequestResult<List<SignatureStatusInfo>> lastTransactions = await Web3.Wallet.ActiveRpcClient.GetSignaturesForAddressAsync(mintAddress.Key, before: beforeSignature);
                        if (lastTransactions.WasSuccessful)
                        {
                            List<SignatureStatusInfo> result = lastTransactions.Result;
                            int count = result.Count;
                            if (count > 0)
                            {
                                int i = count - 1;
                                SignatureStatusInfo lastTransaction = result[i];
                                beforeSignature = lastTransaction.Signature;

                                // We store the oldest block time as the mint creation date,
                                // but since it may fail, and we don't want to get the signatures again,
                                // so we pesimistically iterate our current downloaded transactions
                                // and store its last found time.
                                while (!lastTransaction.BlockTime.HasValue && i > 0)
                                {
                                    lastTransaction = result[--i];
                                }

                                last = DateTime.UnixEpoch.AddSeconds(lastTransaction.BlockTime.Value);
                            }
                            else
                                break;
                        }
                    }

                    if (digimochi.lastMeal == DateTime.UnixEpoch)
                        digimochi.lastMeal = last;

                    if (digimochi.lastMedicine == DateTime.UnixEpoch)
                        digimochi.lastMedicine = last;

                    if (digimochi.lastBath == DateTime.UnixEpoch)
                        digimochi.lastBath = last;
                }

                yield return digimochi;
            }
        }
    }
}