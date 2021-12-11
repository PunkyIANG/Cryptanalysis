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
    const numKnownBits = 40;

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

    foreach (index; parallel((2^^numUnknownBits).iota))
    // int index = 0;
    {
        ulong currentUnsetKey = index;
        ulong currentUnsetKeyMask = 2 ^^ numUnknownBits - 1;

        ulong setKey = knownKeyPart;
        ulong setKeyMask = knownBitMask;

        // writeln("Start: ");
        // writefln("setKey: %016X: ", setKey);
        // writefln("setKeyMask: %016X: ", setKeyMask);
        // writefln("currentUnsetKey: %016X: ", currentUnsetKey);
        // writefln("currentUnsetKeyMask: %016X: ", currentUnsetKeyMask);
        // writeln();
        
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

            // writeln("Step ", currentBit);
            // writefln("setKey: %016X: ", setKey);
            // writefln("setKeyMask: %016X: ", setKeyMask);
            // writefln("currentUnsetKey: %016X: ", currentUnsetKey);
            // writefln("currentUnsetKeyMask: %016X: ", currentUnsetKeyMask);
            // writeln();
        }

        // writeln("Results: ");
        // writefln("setKey: %016X: ", setKey);
        // writefln("setKeyMask: %016X: ", setKeyMask);
        // writefln("currentUnsetKey: %016X: ", currentUnsetKey);
        // writefln("currentUnsetKeyMask: %016X: ", currentUnsetKeyMask);

        // ulong parityBits = des.parityBitsMask;


        bool isKey = true;

        foreach (otherIndex; 0..cleartext.length)
            if (encrypted[otherIndex] != des.encrypt(cleartext[otherIndex], setKey)) {
                isKey = false;
                break;
            }
        
        if (isKey){
            result = setKey;
            atomicOp!"+="(foundKeys, 1);
        }
    }

    writefln("Found key     : %016X", result);
    writefln("Key difference: %016X", result ^ key);
    writeln("Found key count: ", foundKeys);

    if ((key & des.parityBitsMask) != des.getKeyParity(key)) 
        writefln("shit's fucked up: %016X", des.getKeyParity(key));

}
