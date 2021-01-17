x86_64-pc-linux-gnu-g++ -std=c++17 -Wall -fPIC -shared -lusb -o libNavGen.so -Isrc -Ilib/Recast/Include lib/Recast/Source/*.cpp src/*.cpp
