using System;
using System.Collections.Generic;
using Xunit;

namespace igor.repository.sqlserver.integrationTest
{
    public class CollectionTest
    {
        //string connectionString = "mongodb://igortestmongo:DhihwV8N5NNLYFcx5ehzlDrWVYlhx3Kfow7JM2reNDhc74QMTetyO4oCL1iJ1oVhJz493YNe58L0wuzleH7L8w==@igortestmongo.documents.azure.com:10255/[test]?ssl=true";
        string connectionString = "Server=tcp:igortestsqlserver.database.windows.net,1433;Initial Catalog=igortestsqlserver;Persist Security Info=False;User ID=igoradmin;Password=igor$2018;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        [Fact]
        public void NonExistentCollectionTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            //IList<Entity> entities = entityRepository.Find(x => true);
            IList<Entity> entities = entityRepository.Find(null);

            Assert.True(entities != null);
        }

        [Fact]
        public void CleanCollectionTest()
        {
            cleanCollection();

            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            //IList<Entity> entities = entityRepository.Find(x => true);
            IList<Entity> entities = entityRepository.Find(null);

            Assert.True(entities.Count == 0);
        }

        [Fact]
        public void CreateEntityTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            var actual = new Entity() { Id = Guid.NewGuid(), Name = "created" };

             entityRepository.Create(actual);

            var expected = entityRepository.Get(x => x.Id == actual.Id);

            Assert.Equal<Entity>(actual, expected, new EntityEqualityComparer());
        }

        [Fact]
        public void UpdateEntityTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

             entityRepository.Create(initial);

            var actual = entityRepository.Get(x => x.Id == initial.Id);

            actual.Name = "modified";

             entityRepository.Update(actual, x => x.Id == actual.Id);

            var expected = entityRepository.Get(x => x.Id == actual.Id);

            Assert.Equal<Entity>(actual, expected, new EntityEqualityComparer());
        }

        [Fact]
        public void DeleteEntityTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

             entityRepository.Create(initial);

            var deleted = entityRepository.Delete(x => x.Id == initial.Id);

            Assert.True(deleted == 1);
        }

        [Fact]
        public void ExistsEntityTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            var initial = new Entity() { Id = Guid.NewGuid(), Name = "created" };

             entityRepository.Create(initial);

            var actual = entityRepository.Exists(x => x.Id == initial.Id);

            Assert.True(actual);

            var deleted = entityRepository.Delete(x => x.Id == initial.Id);

            actual = entityRepository.Exists(x => x.Id == initial.Id);

            Assert.True(!actual);
        }

        [Fact]
        public void FindEntityTest()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            //var list = entityRepository.Find(x => true);
            var list = entityRepository.Find(null);
            var actual = list.Count;

            for (int i = 0; i < 10; i++)
            {
                entityRepository.Create(new Entity() { Id = Guid.NewGuid(), Name = "created" });
            }

            //list = entityRepository.Find(x => true);
            list = entityRepository.Find(null);
            var expected = list.Count;

            Assert.Equal(actual + 10, expected);
        }

        private void cleanCollection()
        {
            IRepositoryFactory factory = new SqlServerRepositoryFactory(connectionString);

            IRepository<Entity> entityRepository = factory.CreateRepository<Entity>("testCollection");

            //var list = entityRepository.Find(x => true);
            var list = entityRepository.Find(null);

            foreach (var entity in list)
            {
                entityRepository.Delete(x => x.Id == entity.Id);
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
