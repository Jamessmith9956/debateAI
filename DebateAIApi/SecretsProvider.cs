using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using System;

namespace DebateAIApi
{
    // switch to user secrets in development, do this via a IServiceProvider
    public class SecretsProvider
    {
        private readonly SecretClient _secretClient;
        private readonly Dictionary<string, string> _Secrets;

        public SecretsProvider(string keyVaultUri)
        {
            _secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
            _Secrets = new Dictionary<string, string>();
            CacheSecrets();
        }

        private void CacheSecrets()
        {
            foreach (var secret in _secretClient.GetPropertiesOfSecrets())
            {
                _Secrets.Add(secret.Name, _secretClient.GetSecret(secret.Name).Value.Value);
            }
        }

        public string GetSecret(string secretName)
        {
            if (_Secrets.ContainsKey(secretName))
            {
                return _Secrets[secretName];
            }
            else
            {
                var secret = _secretClient.GetSecret(secretName);
                if (secret != null)
                {
                    _Secrets.Add(secretName, secret.Value.Value);
                    return secret.Value.Value;
                }
                else 
                {
                    throw new Exception($"Secret {secretName} not found");
                }
            }
        }
    }
}
