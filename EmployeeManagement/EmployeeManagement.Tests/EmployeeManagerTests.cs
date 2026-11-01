namespace EmployeeManagement.Tests
{
    public class EmployeeManagerTests
    {

        [Fact]
        public void AddEmployee_DifferentIds_BothAdded()
        {
            var manager = new EmployeeManager();
            Assert.True(manager.AddEmployee(new Employee(1, "Roma", "IT", DateTime.Now, 1000m)));
            Assert.True(manager.AddEmployee(new Employee(2, "Mariam", "Finances", DateTime.Now, 1500m)));

            Assert.Equal(2, manager.GetAll().Count);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Employee_NonPositiveId_Throws(int id)
        {
            Assert.Throws<ArgumentException>(() => new Employee(id, "Roma", "IT", DateTime.Now, 1000m));
        }

        [Fact]
        public void SearchEmployee_ByUnknownId_ReturnsNull()
        {
            var manager = new EmployeeManager();
            manager.AddEmployee(new Employee(1, "Roma", "IT", DateTime.Now, 1000m));
            Assert.Null(manager.SearchById(100));
        }

        [Fact]
        public void AddEmployee_DuplicateId_ReturnsFalse_AndDoesNotAdd()
        {
            var manager = new EmployeeManager();

            manager.AddEmployee(new Employee(1, "Roma", "IT", DateTime.Now, 1000m));
            Assert.False(manager.AddEmployee(new Employee(1, "Roma", "Finances", DateTime.Now, 1000m)));
            Assert.Single(manager.GetAll());
            Assert.Equal("IT", manager.SearchById(1)?.Department);
        }

        [Theory]
        [InlineData(-1000)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void NegativeSalary_Throws(decimal salary)
        {
            Assert.Throws<ArgumentException>(() => new Employee(1, "Roma", "IT", DateTime.Now, salary));
        }

        [Fact]
        public void GetTopThree_ReturnsHighestThreeInOrder()
        {
            var manager = new EmployeeManager();
            manager.AddEmployee(new Employee(1, "Roma", "IT", DateTime.Now, 1000m));
            manager.AddEmployee(new Employee(2, "Nika", "Finances", DateTime.Now, 1500m));
            manager.AddEmployee(new Employee(3, "Mariami", "HR", DateTime.Now, 1800m));
            manager.AddEmployee(new Employee(4, "Nini", "Supply", DateTime.Now, 1200m));

            Assert.Equal(new[] {3, 2, 4}, manager.GetTopThree().Select(e => e.Id));
        }



    }
}
