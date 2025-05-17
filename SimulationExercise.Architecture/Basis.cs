namespace SimulationExercise.Architecture
{
    public class Basis
    {
        public Basis(long basisId, string basisCode, string basisDescription)
        {
            BasisId = basisId;
            BasisCode = basisCode;
            BasisDescription = basisDescription;
        }

        public long BasisId { get; set; }
        public string BasisCode { get; set; }
        public string BasisDescription { get; set; }
    }
}
