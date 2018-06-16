namespace Blockche.Wallet.Web.Controllers
{
    using System.Text;

    using Microsoft.AspNetCore.Mvc;

    using Nethereum.Hex.HexConvertors.Extensions;
    using Nethereum.Signer;
    using Nethereum.Signer.Crypto;
    using Nethereum.Util;

    public class AccountController : Controller
    {
        public IActionResult GenerateAccount()
        {
            var sb = new StringBuilder();
            //var privKey = EthECKey.GenerateKey();
            var privKey = new EthECKey("a9f71d3bb9d74a3dd09e9e31533e04637be51aefcfda69a4fbbb5a237547f3");
            var pubKeyCompressed = new ECKey(privKey.GetPrivateKeyAsBytes(), true).GetPubKey(true);
            var pkiMessage = $"Private key: {privKey.GetPrivateKey().Substring(4)}";
            sb.AppendLine(pkiMessage);
            var pubKeyMessage = $"Public key: {privKey.GetPubKey().ToHex().Substring(2)}";
            sb.AppendLine(pubKeyMessage);
            var compressedPubKeyMessage = $"Public key (compressed): {pubKeyCompressed.ToHex()}";
            sb.AppendLine(compressedPubKeyMessage);

            var address = privKey.GetPublicAddress();
            var addressMessage = $"Public address: {address}";
            sb.AppendLine(addressMessage);

            sb.AppendLine();

            var msg = "Blockche";
            sb.AppendLine();
            var msgBytes = Encoding.UTF8.GetBytes(msg);
            var msgHash = new Sha3Keccack().CalculateHash(msgBytes);
            var signature = privKey.SignAndCalculateV(msgHash);
            var signMessage = $"Msg: {msg}";
            sb.AppendLine(signMessage);
            var msgHashMessage = $"Msg hash: {msgHash.ToHex()}";
            sb.AppendLine(msgHashMessage);
            var signatureMessage =
                $"Signature: [v = {signature.V[0] - 27}, r = {signature.R.ToHex()}, s = {signature.S.ToHex()}]";
            sb.AppendLine(signatureMessage);

            var pubKeyRecovered = EthECKey.RecoverFromSignature(signature, msgHash);
            var recoveredPubKeyMessage = $"Recovered pubKey: {pubKeyRecovered.GetPubKey().ToHex().Substring(2)}";
            sb.AppendLine(recoveredPubKeyMessage);

            var validSig = pubKeyRecovered.Verify(msgHash, signature);
            var isValidMessage = $"Signature valid? {validSig}";
            sb.AppendLine(isValidMessage);

            return new ContentResult { Content = sb.ToString() };
        }
    }
}
