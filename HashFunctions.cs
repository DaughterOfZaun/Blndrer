static class HashFunctions
{
    public static uint HashStringFNV1a(string s, uint a2 = 0x811C9DC5)
    {
        uint result = a2;
        for (int i = 0; i < s.Length; i++)
            result = 16777619 * (result ^ s[i]);
        return result;
    }
}