import des;
import std.stdio;
import std.random;
import bitmagic;
import std.range;
import std.parallelism;
import core.atomic : atomicOp;

void main()
{
    // INIT
    ulong[3] cleartext;
    ulong[3] encrypted;
    ulong key;
    const numKnownBits = 35;

    foreach (index; 0 .. cleartext.length)
        cleartext[index] = uniform!ulong;

    key = des.adjustKeyParity(uniform!ulong);

    foreach (index; 0 .. cleartext.length)
    {
        encrypted[index] = des.encrypt(cleartext[index], key);
        writefln("%016X -> %016X", cleartext[index], encrypted[index]);
    }

    writefln("Key           : %016X", key);

    ulong knownBitMask = getRandomMaskWithNSetBits(numKnownBits, des.parityBitsMask, 8) | des
        .parityBitsMask;

    ulong knownKeyPart = key & knownBitMask;

    // writefln("Known bit mask: %016X: ", knownBitMask);
    // writefln("Known key part: %016X: ", knownKeyPart);
    writefln("Des bit mask  : %016X: ", des.parityBitsMask);
    // writeln();

    // key = 0;

    shared ulong result = 0;
    ulong one = 1;

    int numUnknownBits = 56 - numKnownBits;
    shared int foundKeys = 0;

    foreach (index; parallel((2 ^^ numUnknownBits).iota))
    {
        ulong currentUnsetKey = index;
        ulong currentUnsetKeyMask = 2 ^^ numUnknownBits - 1;

        ulong setKey = knownKeyPart;
        ulong setKeyMask = knownBitMask;

        foreach (currentBit; 0 .. 64)
        {
            if (setKeyMask & (one << currentBit))
            {
                currentUnsetKey <<= 1;
                currentUnsetKeyMask <<= 1;

                continue;
            }

            setKey |= currentUnsetKey & (one << currentBit);
            setKeyMask |= one << currentBit;

            currentUnsetKey &= ~(one << currentBit);
            currentUnsetKeyMask &= ~(one << currentBit);

        }

        if (!(setKey & des.parityBitsMask) != des.getKeyParity(setKey))
        {
            bool isKey = true;

            foreach (otherIndex; 0 .. cleartext.length)
                if (encrypted[otherIndex] != des.encrypt(cleartext[otherIndex], setKey))
                {
                    isKey = false;
                    break;
                }

            if (isKey)
            {
                result = setKey;
                atomicOp!"+="(foundKeys, 1);
            }
        }
    }

    writefln("Found key     : %016X", result);
    writefln("Key difference: %016X", result ^ key);
}
