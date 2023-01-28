﻿using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Imagibee {
    namespace Gigantor {
        public class Utilities
        {
            // Avoid showing username for privacy concerns
            public static string RemoveUsername(string path)
            {
                // mac format
                return Regex.Replace(path, @"/Users/([^/]*/)", "~/");
                // TODO: add more formats
            }

            public static long FileByteCount(IEnumerable<string> paths)
            {
                long byteCount = 0;
                foreach (var path in paths)
                {
                    byteCount += FileByteCount(path);
                }
                return byteCount;
            }

            public static long FileByteCount(string path)
            {
                FileInfo fileInfo = new(path);
                return fileInfo.Length;
            }

            // from https://www.techmikael.com/2009/01/fast-byte-array-comparison-in-c.html
            // NOTE: for UtilitiesTests.UnsafeIsEqualTest this worked but in practice it
            // failed for very large files.  Its very fast so would be nice to understand
            // if it is a bug that can be fixed or not.
            public static unsafe bool UnsafeIsEqualBroken(byte[] strA, byte[] strB)
            {
                int length = strA.Length;
                if (length != strB.Length) {
                    return false;
                }
                fixed (byte* str = strA) {
                    byte* chPtr = str;
                    fixed (byte* str2 = strB) {
                        byte* chPtr2 = str2;
                        byte* chPtr3 = chPtr;
                        byte* chPtr4 = chPtr2;
                        while (length >= 10) {
                            if ((((*(((int*)chPtr3)) != *(((int*)chPtr4))) || (*(((int*)(chPtr3 + 2))) != *(((int*)(chPtr4 + 2))))) || ((*(((int*)(chPtr3 + 4))) != *(((int*)(chPtr4 + 4)))) || (*(((int*)(chPtr3 + 6))) != *(((int*)(chPtr4 + 6)))))) || (*(((int*)(chPtr3 + 8))) != *(((int*)(chPtr4 + 8))))) {
                                break;
                            }
                            chPtr3 += 10;
                            chPtr4 += 10;
                            length -= 10;
                        }
                        while (length > 0) {
                            if (*(((int*)chPtr3)) != *(((int*)chPtr4))) {
                                break;
                            }
                            chPtr3 += 2;
                            chPtr4 += 2;
                            length -= 2;
                        }
                        return (length <= 0);
                    }
                }
            }

            // Returns true if all elements of value1 and value2 are equal 
            public static bool UnsafeIsEqual(byte[] value1, byte[] value2)
            {
                if (IntPtr.Size == 4) {
                    return UnsafeIsEqual32(value1, value2);
                }
                else if (IntPtr.Size == 8) {
                    return UnsafeIsEqual64(value1, value2);
                }
                else {
                    throw new NotImplementedException();
                }
            }

            protected static unsafe bool UnsafeIsEqual32(byte[] value1, byte[] value2)
            {
                var length = value1.Length;
                if (length != value2.Length) {
                    return false;
                }
                fixed (byte* p1 = value1, p2 = value2) {
                    int* lp1 = (int*)p1;
                    int* lp2 = (int*)p2;
                    for (var i = 0; i < length / sizeof(int); i += 1) {
                        if (lp1[i] != lp2[i]) {
                            return false;
                        }
                    }
                }
                return true;
            }

            protected static unsafe bool UnsafeIsEqual64(byte[] value1, byte[] value2)
            {
                var length = value1.Length;
                if (length != value2.Length) {
                    return false;
                }
                fixed (byte* p1 = value1, p2 = value2) {
                    long* lp1 = (long*)p1;
                    long* lp2 = (long*)p2;
                    for (var i = 0; i < length / sizeof(long); i++) {
                        if (lp1[i] != lp2[i]) {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}