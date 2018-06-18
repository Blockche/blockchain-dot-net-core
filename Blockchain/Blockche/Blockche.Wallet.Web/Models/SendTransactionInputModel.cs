namespace Blockche.Wallet.Web.Models
{
    public class SendTransactionInputModel : SignTransactionInputModel
    {
        public string[] Signature{ get; set; }
    }
}
