// ==========================================================================
// LocalizationManagerTest.cs
// Green Parrot Framework
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Globalization;
using GP.Utils;
using Rhino.Mocks;
using Tests.Given;
using Xunit;

namespace Tests.Facts
{
    public class LocalizationManagerTest : GivenMocks
    {
        private readonly ILocalizationProvider provider;

        public LocalizationManagerTest()
        {
            provider = Mocks.StrictMock<ILocalizationProvider>();

            LocalizationManager.Provider = provider;
        }

        [Fact]
        public void GetFormattedString_ResourceKeyIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => LocalizationManager.GetFormattedString(null));
        }

        [Fact]
        public void GetFormattedString_ResourceKeyIsEmpty_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => LocalizationManager.GetFormattedString(string.Empty));
        }

        [Fact]
        public void GetFormattedString_NoResourceProvider_ThrowsException()
        {
            LocalizationManager.Provider = null;

            Assert.Throws<InvalidOperationException>(() => LocalizationManager.GetFormattedString("Key"));
        }

        [Fact]
        public void GetFormattedString_FromProvider_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo");
            }

            using (Playback())
            {
                string result = LocalizationManager.GetFormattedString("Key1");

                Assert.Equal("Foo", result);
            }
        }

        [Fact]
        public void GetFormattedString_FromProviderWithArguments_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo {0}");
            }

            using (Playback())
            {
                string result = LocalizationManager.GetFormattedString("Key1", 123);

                Assert.Equal("Foo 123", result);
            }
        }

        [Fact]
        public void GetFormattedString_FormatException_PassedThrough()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("{0} {1}");
            }

            using (Playback())
            {
                Assert.Throws<FormatException>(() => LocalizationManager.GetFormattedString("Key1", 123));
            }
        }

        [Fact]
        public void GetFormattedString_ProviderThrowsException_PassedThrough()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Throw(new InvalidCastException());
            }

            using (Playback())
            {
                Assert.Throws<InvalidCastException>(() => LocalizationManager.GetFormattedString("Key1"));
            }
        }

        [Fact]
        public void GetFormattedString_ProviderReturnsNull_ThrowsException()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return(null);
            }

            using (Playback())
            {
                Assert.Throws<ArgumentException>(() => LocalizationManager.GetFormattedString("Key1"));
            }
        }

        [Fact]
        public void GetString_ResourceKeyIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => LocalizationManager.GetString(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void GetString_ResourceKeyIsEmpty_ThrowsException(string key)
        {
            Assert.Throws<ArgumentException>(() => LocalizationManager.GetString(key));
        }

        [Fact]
        public void GetString_NoResourceProvider_ThrowsException()
        {
            LocalizationManager.Provider = null;

            Assert.Throws<InvalidOperationException>(() => LocalizationManager.GetString("Key"));
        }

        [Fact]
        public void GetString_FromProvider_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo");
            }

            using (Playback())
            {
                string result = LocalizationManager.GetString("Key1");

                Assert.Equal("Foo", result);
            }
        }

        [Fact]
        public void GetString_ProviderThrowsException_PassedThrough()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Throw(new InvalidCastException());
            }

            using (Playback())
            {
                Assert.Throws<InvalidCastException>(() => LocalizationManager.GetString("Key1"));
            }
        }

        [Fact]
        public void GetString_ProviderReturnsNull_ThrowsException()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return(null);
            }

            using (Playback())
            {
                Assert.Throws<ArgumentException>(() => LocalizationManager.GetString("Key1"));
            }
        }

        [Fact]
        public void TryGetString_ResourceKeyIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => LocalizationManager.TryGetString(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void TryGetString_ResourceKeyIsEmpty_ThrowsException(string key)
        {
            Assert.Throws<ArgumentException>(() => LocalizationManager.TryGetString(key));
        }

        [Fact]
        public void TryGetString_NoResourceProvider_ReturnsKey()
        {
            LocalizationManager.Provider = null;

            string result = LocalizationManager.TryGetString("Key");

            Assert.Equal("Key", result);
        }

        [Fact]
        public void TryGetString_FromProvider_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo");
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetString("Key1");

                Assert.Equal("Foo", result);
            }
        }

        [Fact]
        public void TryGetString_ProviderThrowsException_ReturnsKey()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Throw(new InvalidCastException());
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetString("Key1");

                Assert.Equal("Key1", result);
            }
        }
        [Fact]
        public void TryGetFormattedString_ResourceKeyIsNull_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => LocalizationManager.TryGetFormattedString(null));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void TryGetFormattedString_ResourceKeyIsEmpty_ThrowsException(string key)
        {
            Assert.Throws<ArgumentException>(() => LocalizationManager.TryGetFormattedString(key));
        }

        [Fact]
        public void TryGetFormattedString_NoResourceProvider_ThrowsException()
        {
            LocalizationManager.Provider = null;

            string result = LocalizationManager.TryGetFormattedString("Key");

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void TryGetFormattedString_FromProvider_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo");
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetFormattedString("Key1");

                Assert.Equal("Foo", result);
            }
        }

        [Fact]
        public void TryGetFormattedString_FromProviderWithArguments_ReturnsValue()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("Foo {0}");
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetFormattedString("Key1", 123);

                Assert.Equal("Foo 123", result);
            }
        }

        [Fact]
        public void TryGetFormattedString_FormatException_ReturnsEmpty()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Return("{0} {1}");
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetFormattedString("Key1", 123);

                Assert.Equal(string.Empty, result);
            }
        }

        [Fact]
        public void TryGetFormattedString_ProviderThrowsException_ReturnsEmpty()
        {
            using (Record())
            {
                provider.Expect(x => x.GetString("Key1", CultureInfo.CurrentCulture)).Throw(new InvalidCastException());
            }

            using (Playback())
            {
                string result = LocalizationManager.TryGetFormattedString("Key1", "Fallback", new object[0]);

                Assert.Equal(string.Empty, result);
            }
        }
    }
}
