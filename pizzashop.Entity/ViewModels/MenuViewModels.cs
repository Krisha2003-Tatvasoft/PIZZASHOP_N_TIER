namespace pizzashop.Entity.ViewModels;
using VMCategory = pizzashop.Entity.ViewModels.Category;
using VMMG = pizzashop.Entity.ViewModels.ModifiersGroup;
using VMItemTable = pizzashop.Entity.ViewModels.ItemTable;
using VMModifierTable = pizzashop.Entity.ViewModels.ModifierTable;

public class MenuViewModels
{
    public List<VMCategory> CategoryList {get; set;} 

    public VMCategory category {get; set;}

    public List<VMMG> ModifiersGroups {get; set;}

    public List<VMItemTable> ItemTable {get; set;}

     public List<VMModifierTable> ModifierTable {get; set;}

}
