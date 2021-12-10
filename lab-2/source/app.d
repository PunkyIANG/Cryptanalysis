import des;
import std.stdio;
import std.random;
import bitmagic;

void main()
{
    // INIT
    ulong[3] cleartext;
    ulong[3] encrypted;
    ulong key;
    const numKnownBits = 40;

    foreach (index; 0..cleartext.length)
        cleartext[index] = uniform!ulong;

    key = des.adjustKeyParity(uniform!ulong);


    foreach (index; 0..cleartext.length) {
        encrypted[index] = des.encrypt(cleartext[index], key);
        writefln("%016X -> %016X", cleartext[index], encrypted[index]);
    }

    writefln("Key: %016X", key);

    ulong knownBitMask = getRandomMaskWithNSetBits(numKnownBits, des.parityBitsMask, 8) | des.parityBitsMask;    

    ulong knownKeyPart = key & knownBitMask;

    key = 0;



    

}
