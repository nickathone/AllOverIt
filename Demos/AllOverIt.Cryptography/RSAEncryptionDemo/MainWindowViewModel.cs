using AllOverIt.Cryptography.Extensions;
using AllOverIt.Cryptography.Hybrid;
using AllOverIt.Cryptography.RSA;
using AllOverIt.Reactive;
using System;
using System.IO;
using System.Text;

namespace RSAEncryptionDemo
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly IRsaEncryptor _encryptor;

        private string _publicKey;
        public string PublicKey
        {
            get => _publicKey;
            private set => RaiseAndSetIfChanged(ref _publicKey, value);
        }

        private string _privateKey;
        public string PrivateKey
        {
            get => _privateKey;
            private set => RaiseAndSetIfChanged(ref _privateKey, value);
        }

        private string _textInput;
        public string TextInput
        {
            get => _textInput;
            set => RaiseAndSetIfChanged(ref _textInput, value, null, OnTextInputChanged);
        }

        private int _maxInputLength;
        public int MaxInputLength
        {
            get => _maxInputLength;
            private set => RaiseAndSetIfChanged(ref _maxInputLength, value);
        }

        private string _textEncrypted;
        public string TextEncrypted
        {
            get => _textEncrypted;
            private set => RaiseAndSetIfChanged(ref _textEncrypted, value);
        }

        private string _textDecrypted;
        public string TextDecrypted
        {
            get => _textDecrypted;
            private set => RaiseAndSetIfChanged(ref _textDecrypted, value);
        }

        public MainWindowViewModel()
        {
            // Creates a new public/private key pair with 128-bit security
            var rsaKeyPair = RsaKeyPair.Create();

            _encryptor = RsaEncryptor.Create(rsaKeyPair);

            PublicKey = Convert.ToBase64String(rsaKeyPair.PublicKey);
            PrivateKey = Convert.ToBase64String(rsaKeyPair.PrivateKey);
            MaxInputLength = _encryptor.GetMaxInputLength();
            TextInput = $"Enter some text here to see it encrypted (max length {MaxInputLength} bytes)";





            //var configuration = new RsaAesHybridEncryptorConfiguration
            //{
            //    Encryption = new RsaEncryptionConfiguration
            //    {
            //        Keys = rsaKeyPair,
            //        Padding = RSAEncryptionPadding.OaepSHA256
            //    },

            //    Signing = new RsaSigningConfiguration
            //    {
            //        HashAlgorithmName = HashAlgorithmName.SHA256,
            //        Padding = RSASignaturePadding.Pkcs1
            //    }
            //};


            // Same as above due to defaults
            var configuration = new RsaAesHybridEncryptorConfiguration
            {
                Encryption = new RsaEncryptionConfiguration(rsaKeyPair)
            };



            var e1 = new RsaAesHybridEncryptor(configuration);
            var hybridEncrypted1 = e1.EncryptPlainTextToBytes(TextInput);
            var hybridDecrypted1 = e1.DecryptBytesToPlainText(hybridEncrypted1);



            var e2 = new RsaAesHybridEncryptor(configuration);
            var ms1 = new MemoryStream(e2.EncryptPlainTextToBytes(TextInput));
            var ms2 = new MemoryStream();
            e2.Decrypt(ms1, ms2);
            ms2.Position = 0;
            var hybridDecrypted2 = Encoding.UTF8.GetString(ms2.ToArray());    //e2.DecryptBytesToPlainText(ms.ToArray());



            var b = e2.EncryptStreamToBytes(new MemoryStream(Encoding.UTF8.GetBytes(TextInput)));       // encrypt plain text in a stream
            var t1 = e2.DecryptBytesToPlainText(b);

            var ms3 = new MemoryStream();
            e2.DecryptBytesToStream(b, ms3);
            var t2 = Encoding.UTF8.GetString(ms3.ToArray());


        }

        private void OnTextInputChanged()
        {
            TextEncrypted = _encryptor.EncryptPlainTextToBase64(TextInput);
            TextDecrypted = _encryptor.DecryptBase64ToPlainText(TextEncrypted);
        }
    }
}
