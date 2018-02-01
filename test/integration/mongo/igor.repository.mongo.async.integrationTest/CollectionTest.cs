using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace igor.repository.mongo.async.integrationTest
{
    public class CollectionTest
    {        
        string connectionString = "mongodb://igortestmongo:DhihwV8N5NNLYFcx5ehzlDrWVYlhx3Kfow7JM2reNDhc74QMTetyO4oCL1iJ1oVhJz493YNe58L0wuzleH7L8w==@igortestmongo.documents.azure.com:10255/[test]?ssl=true";

        [Fact]
        public async Task NonExistentCollectionTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");
            
            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            IList<Entity> entities = await entityRepository.Find(x => true);
            
            Assert.True(entities != null);
        }

        [Fact]
        public async Task CleanCollectionTest()
        {
            await cleanCollection();

            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            IList<Entity> entities = await entityRepository.Find(x => true);

            Assert.True(entities.Count == 0);
        }

        [Fact]
        public async Task CreateEntityTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var actual = new Entity() { Id = Guid.NewGuid(), Name = "created" };

            await entityRepository.Create(actual);

            var expected = await entityRepository.Get(x => x.Id == actual.Id);

            Assert.Equal<Entity>(actual, expected, new EntityEqualityComparer());
        }

        [Fact]
        public async Task UpdateEntityTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

            await entityRepository.Create(initial);
            
            var actual = await entityRepository.Get(x => x.Id == initial.Id);

            actual.Name = "modified";

            await entityRepository.Update(actual, x => x.Id == actual.Id);

            var expected = await entityRepository.Get(x => x.Id == actual.Id);

            Assert.Equal<Entity>(actual, expected, new EntityEqualityComparer());
        }

        [Fact]
        public async Task DeleteEntityTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

            await entityRepository.Create(initial);

            var deleted = await entityRepository.Delete(x => x.Id == initial.Id);

            Assert.True(deleted == 1);
        }

        [Fact]
        public async Task ExistsEntityTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

            await entityRepository.Create(initial);

            var actual = await entityRepository.Exists(x => x.Id == initial.Id);

            Assert.True(actual);

            var deleted = await entityRepository.Delete(x => x.Id == initial.Id);

            actual = await entityRepository.Exists(x => x.Id == initial.Id);

            Assert.True(!actual);
        }

        [Fact]
        public async Task FindEntityTest()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var list = await entityRepository.Find(x => true);
            var actual = list.Count;

            for (int i = 0; i < 10; i++)
            {
                await entityRepository.Create(new Entity() { Id = Guid.NewGuid(), Name = "created" });
            }

            list = await entityRepository.Find(x => true);
            var expected = list.Count;

            Assert.Equal(actual + 10, expected);
        }

        private async Task cleanCollection()
        {
            IAsyncRepositoryFactory factory = new MongoAsyncRepositoryFactory(connectionString, "testDb");

            IAsyncRepository<Entity> entityRepository = factory.CreateAsyncRepository<Entity>("testCollection");

            var list = await entityRepository.Find(x => true);

            foreach(var entity in list)
            {
                await entityRepository.Delete(x => x.Id == entity.Id);
            }
        }

        class Entity
        {
            public Guid Id { get; set; }
            public String Name { get; set; }
        }

        class EntityEqualityComparer : IEqualityComparer<Entity>
        {
            public bool Equals(Entity b1, Entity b2)
            {
                if (b2 == null && b1 == null)
                    return true;
                else if (b1 == null | b2 == null)
                    return false;
                else if (b1.Id == b2.Id && b1.Name == b2.Name)
                    return true;
                else
                    return false;
            }

            public int GetHashCode(Entity bx)
            {
                int hCode = bx.Id.ToString().Length ^ bx.Name.Length;
                return hCode.GetHashCode();
            }
        }
    }
}
