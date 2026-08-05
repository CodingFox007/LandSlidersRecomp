using System;

[Flags]
public enum DiffusionPlatform
{
	Custom = 1,
	Facebook = 2,
	Twitter = 4,
	Weibo = 8,
	Message = 0x10,
	Mail = 0x20,
	Print = 0x40,
	Copy = 0x80,
	AssignToContact = 0x100,
	SaveToCameraRoll = 0x200,
	ReadingList = 0x400,
	Flickr = 0x800,
	Vimeo = 0x1000,
	TencentWeibo = 0x2000,
	AirDrop = 0x4000
}
