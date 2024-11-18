1) In scene search ItemMall open and select ItemMallPanel
    1.1) in inspector add new component ItemRarity and select Slot Type Item Mall
    1.2) change item slot prefab to the one of the same name contained in the ItemRatity/Prefab addon folder


5) In scene search Loot open and select LootPanel
    2.1) in inspector add new component ItemRarity and select Slot Type Loot
    2.2) change item slot prefab to the one of the same name contained in the ItemRatity/Prefab addon folder


3) Open UiItemMall.cs

replace 
    int currentCategory = 0;

to
    public int currentCategory = 0;