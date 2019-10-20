namespace Traingles.DomainModel.TriangleFinder
{
    public class RequestedTriangle
    {
        public int Column { get; set; }
        public RowEnum Row { get; set; }
        public string sRow { get; set; }

        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
        public int X3 { get; set; }
        public int Y3 { get; set; }

        public bool IsEstimated { get; set; }
    }
}