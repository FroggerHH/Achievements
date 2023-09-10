namespace Extensions.Valheim;

public static class ItemDropExtension
{
    public static string LocalizeName(this ItemDrop drop) { return drop.m_itemData.LocalizeName(); }

    public static string LocalizeName(this ItemDrop.ItemData drop) { return drop.m_shared.LocalizeName(); }

    public static string LocalizeName(this ItemDrop.ItemData.SharedData drop) { return drop.m_name.Localize(); }
}