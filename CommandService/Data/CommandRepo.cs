using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
            
        }
        public void CreateCommand(int platformId, Command command)
        {
            if(command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform  == null)
            {
                throw new ArgumentNullException(nameof(platform));
            }
             _context.Platforms.Add(platform);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
           return _context.Platforms.Any(x => x.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatform()
        {
           return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
           return _context.Commands
                 .Where(x => x.PlatformId == platformId && x.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandForPlatform(int platfromId)
        {
           return _context.Commands
                   .Where(x => x.PlatformId == platfromId)
                   .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExist(int platformId)
        {
            return _context.Platforms.Any(x => x.Id == platformId);
        }

        public bool SaveChanges()
        {
           return (_context.SaveChanges() >= 0);
        }
    }
}