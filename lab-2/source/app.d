static import des;
import std.stdio;
import std.random;


ulong getRandomBitMask(int knownBitCount) {
    import std.container : SList;
    
    assert(knownBitCount > 0);
    assert(knownBitCount < ulong.sizeof * 8);

    ulong result;

    auto series = SList!int(0);
    int length = ulong.sizeof * 8;
    
    foreach (int index; 1..length)
        series.insertFront(index);

    

    foreach (index; 0..knownBitCount)
    {
        int selectedIndex = uniform(0, length);
        length--;

        foreach (key; list)
        {
            
        }
    }
    

    return result;
}

void main()
{
    // INIT
    ulong[3] cleartext;
    ulong[3] encrypted;
    ulong key;

    foreach (index; 0..cleartext.length)
        cleartext[index] = uniform!ulong;

    key = des.adjustKeyParity(uniform!ulong);


    foreach (index; 0..cleartext.length) {
        encrypted[index] = des.encrypt(cleartext[index], key);
        writefln("%016X -> %016X", cleartext[index], encrypted[index]);
    }

    writefln("Key: %016X", key);




    key = 0;

}
