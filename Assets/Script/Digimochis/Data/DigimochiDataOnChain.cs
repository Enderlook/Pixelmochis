using System;

public class DigimochiOnChainData : IDigimochiData
{
    // Placeholder para datos de la blockchain
    // Aquí irá la lógica para recuperar los datos de la blockchain

    public string GetName()
    {
        // Implementar lógica para obtener el nombre desde la blockchain
        return "NombreDesdeBlockchain";
    }

    public int GetDigimochiType()
    {
        // Implementar averiguar que tipo de digimochi es (que especie).
        return 0;
    }

    public DateTime GetLastBathTime()
    {
        // Implementar lógica para obtener el último baño desde la blockchain
        // El formato de la fecha y hora debería ser "ddMMyyyyHHmmss"
        // Esto es solo un placeholder. Deberías reemplazarlo por la lógica real para obtener el dato desde la blockchain.
        return DateTime.UnixEpoch;
    }

    public DateTime GetLastMedicineTime()
    {
        // Implementar lógica para obtener la última vez que se le dio medicina desde la blockchain
        // El formato de la fecha y hora debería ser "ddMMyyyyHHmmss"
        // Esto es solo un placeholder. Deberías reemplazarlo por la lógica real para obtener el dato desde la blockchain.
        return DateTime.UnixEpoch;
    }
}
