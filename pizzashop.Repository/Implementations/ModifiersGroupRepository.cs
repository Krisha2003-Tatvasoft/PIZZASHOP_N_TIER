using pizzashop.Entity.Models;
using pizzashop.Repository.Interfaces;

namespace pizzashop.Repository.Implementations;

public class ModifiersGroupRepository:IModifiersGroupRepository
{
      private readonly PizzashopContext _context;

        public ModifiersGroupRepository(PizzashopContext context)
        {
            _context = context;
        }

         public List<Modifiergroup> AllModifiersGroup()
    {
          return  _context.Modifiergroups.ToList();
    }
}
