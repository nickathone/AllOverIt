using AllOverIt.Encryption;
using AllOverIt.Encryption.RSA;
using AllOverIt.Encryption.RSA.Extensions;
using AllOverIt.Reactive;
using System;

namespace RSAEncryptionDemo
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly RSAKeyPair _rsaKeyPair;
        private readonly RSAEncrypter _encrypter;

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
            _rsaKeyPair = RSAKeyGenerator.CreateKeyPair();

            _encrypter = new RSAEncrypter(_rsaKeyPair);

            PublicKey = Convert.ToBase64String(_rsaKeyPair.PublicKey);
            PrivateKey = Convert.ToBase64String(_rsaKeyPair.PrivateKey);

            TextInput = $"Enter some text here to see it encrypted (max length {_encrypter.MaxInputLength} bytes)";
        }

        private void OnTextInputChanged()
        {
            TextEncrypted = _encrypter.EncryptTextToBase64(TextInput);
            TextDecrypted = _encrypter.DecryptBase64ToText(TextEncrypted);
        }
    }
}
