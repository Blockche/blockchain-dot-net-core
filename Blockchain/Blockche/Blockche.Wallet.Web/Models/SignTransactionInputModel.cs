namespace Blockche.Wallet.Web.Models
{
    public class SignTransactionInputModel
    {
        public string RecipientAddress { get; set; }

        public int Value { get; set; }

        public int Fee { get; set; }

        public string PrivateKey { get; set; }
    }
}
