using AllOverIt.Cryptography.AES;
using AllOverIt.Cryptography.Extensions;
using AllOverIt.Reactive;
using System;

namespace AESEncryptionDemo
{
    public sealed class MainWindowViewModel : ObservableObject
    {
        private readonly IAesEncryptor _encryptor;

        private string _key;
        public string Key
        {
            get => _key;
            private set => RaiseAndSetIfChanged(ref _key, value);
        }

        private string _iv;
        public string IV
        {
            get => _iv;
            private set => RaiseAndSetIfChanged(ref _iv, value);
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
            _encryptor = new AesEncryptor();

            Key = Convert.ToBase64String(_encryptor.Key);
            IV = Convert.ToBase64String(_encryptor.IV);
            TextInput = $"Enter some text here to see it encrypted";




            //var e1 = new AESEncryptor();

            //var len = e1.GetCipherTextLength(3);

            //var encrypted1 = e1.Encrypt(new byte[] { 1, 2, 3 });
            //var decrypted1 = e1.Decrypt(encrypted1);


            //var e2 = new AESEncryptor();
            //var m1 = new MemoryStream(new byte[] { 1, 2, 3 });
            //var m2 = new MemoryStream();

            //e2.Encrypt(m1, m2);

            //var encrypted2 = m2.ToArray();
            //m1 = new MemoryStream(encrypted2);
            //m2 = new MemoryStream();
            //e2.Decrypt(m1, m2);
            //var decrypted2 = m2.ToArray();


            //var t1 = e2.EncryptPlainTextToBase64("hello world");
            //var t2 = e2.DecryptBase64ToPlainText(t1);




            // TODO: Custom exceptions

            // TODO: Combine RSA + AES

        }

        private void OnTextInputChanged()
        {
            TextEncrypted = _encryptor.EncryptPlainTextToBase64(TextInput);
            TextDecrypted = _encryptor.DecryptBase64ToPlainText(TextEncrypted);
        }
    }
}
