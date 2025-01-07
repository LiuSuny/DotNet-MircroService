using CommandService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();
       //related platform command
        IEnumerable<Platform> GetAllPlatform();
        void CreatePlatform(Platform platform);
        bool PlatformExist(int platformId);
        
       //related command service
        IEnumerable<Command> GetCommandForPlatform(int platfromId);
        Command GetCommand(int platformId, int commandId);
        void CreateCommand(int platformId, Command command);
    }
}