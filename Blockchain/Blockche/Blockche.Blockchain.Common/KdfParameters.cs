namespace Blockche.Blockchain.Common
{
    public class KdfParameters
    {
        public int DkLength { get; set; }

        public string Salt { get; set; }

        public int N { get; set; }

        public int R { get; set; }

        public int P { get; set; }
    }
}
