﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
namespace Api.Helper
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureProtected<TOptions>(this IServiceCollection services,
                        IConfigurationSection section) where TOptions : class, new()
        {
            return services.AddSingleton(provider =>
            {
                var dataProtectionProvider = provider.GetRequiredService<IDataProtectionProvider>();
                section = new ProtectedConfigurationSection(dataProtectionProvider, section);

                var options = section.Get<TOptions>();
                return Options.Create(options);
            });
        }

        private class ProtectedConfigurationSection : IConfigurationSection
        {
            private readonly IDataProtectionProvider _dataProtectionProvider;
            private readonly IConfigurationSection _section;
            private readonly Lazy<IDataProtector> _protector;

            public ProtectedConfigurationSection(
                IDataProtectionProvider dataProtectionProvider,
                IConfigurationSection section)
            {
                _dataProtectionProvider = dataProtectionProvider;
                _section = section;

                _protector = new Lazy<IDataProtector>(() => dataProtectionProvider.CreateProtector(section.Path));
            }

            public IConfigurationSection GetSection(string key)
            {
                return new ProtectedConfigurationSection(_dataProtectionProvider, _section.GetSection(key));
            }

            public IEnumerable<IConfigurationSection> GetChildren()
            {
                return _section.GetChildren()
                    .Select(x => new ProtectedConfigurationSection(_dataProtectionProvider, x));
            }

            public IChangeToken GetReloadToken()
            {
                return _section.GetReloadToken();
            }

            public string this[string key]
            {
                get => GetProtectedValue(_section[key]);
                set => _section[key] = _protector.Value.Protect(value);
            }

            public string Key => _section.Key;
            public string Path => _section.Path;

            public string Value
            {
                get => GetProtectedValue(_section.Value);
                set => _section.Value = _protector.Value.Protect(value);
            }

            private string GetProtectedValue(string value)
            {
                if (value == null)
                    return null;

                return _protector.Value.Unprotect(value);
            }
        }
    }
}
