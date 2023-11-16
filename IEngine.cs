namespace Demographic
{
    public interface IEngine
    {
        List<Person> InitialPopulation(); // Метод запуска моделирования
        void Model();
    }
}