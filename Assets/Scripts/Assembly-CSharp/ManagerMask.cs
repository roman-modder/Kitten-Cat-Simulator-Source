using System;

[Flags]
public enum ManagerMask
{
	None = 0,
	All = 1,
	BadWord = 2,
	Domain = 4,
	Capitalization = 8,
	Punctuation = 0x10
}
