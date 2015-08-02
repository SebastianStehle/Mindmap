// ==========================================================================
// DocumentStoreTest.cs
// RavenMind Application
// ==========================================================================
// Copyright (c) Sebastian Stehle
// All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RavenMind.Model;

namespace RavenMind.Tests
{
    [TestClass]
    public class DocumentStoreTest
    {
        [TestMethod]
        public async Task TestLoadAllAsyncNoEntries()
        {
            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());

            IEnumerable<DocumentRef> refs = await documentStore.LoadAllAsync();

            Assert.AreEqual(0, refs.Count());
        }

        [TestMethod]
        public async Task TestLoadAllAsync()
        {
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());
            await documentStore.StoreAsync(new Document { Name = "Name1", Id = id1 });
            await Task.Delay(5000);
            await documentStore.StoreAsync(new Document { Name = "Name2", Id = id2 });

            IList<DocumentRef> refs = await documentStore.LoadAllAsync();

            Assert.AreEqual(2, refs.Count());
            Assert.AreEqual(id2, refs[0].Id);
            Assert.AreEqual(id1, refs[1].Id);
            Assert.AreEqual("Name2", refs[0].Name);
            Assert.AreEqual("Name1", refs[1].Name);

            await documentStore.ClearAsync();
        }
        
        [TestMethod]
        public async Task TestDeleteAsyncUnknownDocument()
        {
            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());

            await documentStore.DeleteAsync(Guid.NewGuid());
        }

        [TestMethod]
        public void TestConstructor()
        {
            DocumentStore documentStore = new DocumentStore();
        }

        [TestMethod]
        public void TestConstructorWithSubfolder()
        {
            DocumentStore documentStore = new DocumentStore("Subfolder");
        }

        [TestMethod]
        public void TestConstructorWithEmptySubfolderNameException()
        {
            Assert.ThrowsException<ArgumentException>(() => new DocumentStore(string.Empty));
        }

        [TestMethod]
        public void TestConstructorWithNullSubfolderNameException()
        {
            Assert.ThrowsException<ArgumentException>(() => new DocumentStore(null));
        }

        [TestMethod]
        public void TestConstructorWithWhitespaceSubfolderNameException()
        {
            Assert.ThrowsException<ArgumentException>(() => new DocumentStore(" "));
        }

        [TestMethod]
        public async Task TestStoreAsyncNullDocumentException()
        {
            try
            {
                DocumentStore documentStore = new DocumentStore();

                await documentStore.StoreAsync(null);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentNullException));
            }
        }

        [TestMethod]
        public async Task TestStoreAsyncNullNameException()
        {
            try
            {
                DocumentStore documentStore = new DocumentStore();

                await documentStore.StoreAsync(new Document { Name = null });

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
        }

        [TestMethod]
        public async Task TestStoreAsyncEmptyNameException()
        {
            try
            {
                DocumentStore documentStore = new DocumentStore();

                await documentStore.StoreAsync(new Document { Name = string.Empty });

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
        }

        [TestMethod]
        public async Task TestStoreAsyncWhitespaceNameException()
        {
            try
            {
                DocumentStore documentStore = new DocumentStore();

                await documentStore.StoreAsync(new Document { Name = " " });

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
        }

        [TestMethod]
        public async Task TestStoreAsync()
        {
            Guid id = Guid.NewGuid();

            Document document = new Document();
            document.Id = id;
            document.Root.Text = "FOO";
            document.Name = "Name";

            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());

            await documentStore.StoreAsync(document);
            await documentStore.ClearAsync();
        }

        [TestMethod]
        public async Task TestDeleteAsync()
        {
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());
            await documentStore.StoreAsync(new Document { Name = "Name1", Id = id1 });
            await documentStore.StoreAsync(new Document { Name = "Name2", Id = id2 });

            await documentStore.DeleteAsync(id1);

            IList<DocumentRef> refs = await documentStore.LoadAllAsync();

            Assert.AreEqual(1, refs.Count());
            Assert.AreEqual(id2, refs[0].Id);
            Assert.AreEqual("Name2", refs[0].Name);

            await documentStore.ClearAsync();
        }

        [TestMethod]
        public async Task TestLoadAsync()
        {
            Guid id = Guid.NewGuid();

            Document document = new Document();
            document.Id = id;
            document.Root.Text = "FOO";
            document.Name = "Name";
            
            DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());

            await documentStore.StoreAsync(document);

            Document documentLoaded = await documentStore.LoadAsync(id);

            Assert.AreEqual(document.Root.Text, documentLoaded.Root.Text);

            await documentStore.ClearAsync();
        }

        [TestMethod]
        public async Task TestLoadAsyncDocumentNotExistsException()
        {
            try
            {
                DocumentStore documentStore = new DocumentStore(Guid.NewGuid().ToString());

                await documentStore.LoadAsync(Guid.NewGuid());

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(FileNotFoundException));
            }
        }
    }
}
