using System;

public interface SaveType
{
	bool IsType(string valueStr);

	Type GetValueType();

	object FromString(string valueStr);

	string ToString(object obj);
}
