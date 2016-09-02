
// This is auto-generated code. Please, edit only method bodies.
    
public static byte[] ReadFromMemory(CPU cpu, uint address)
{
    // Define how CPU reads from memory here. 
    // If this method is not needed, just leave it empty.
	cpu.GetPort("AD").Val = (int)address;
	cpu.GetPort("RD").Val = 1;
	cpu.Wait(2);
	cpu.GetPort("RD").Val = 0;
	cpu.Wait(13);
	//return Program.Mem[address];
	int readValue = cpu.GetPort("AD").Val;
	byte[] binaryValue = ConversionHelper.ConvertFromIntToByteArray(cpu.GetPort("AD").Val, cpu.GetPort("AD").Size);
	cpu.Wait(3);
	return binaryValue;
}

public static void WriteToMemory(CPU cpu, uint address, byte[] value)
{
    // Define how CPU writtes to memory here.
    // If this method is not needed, just leave it empty.
}

public static void CheckForInterrupts(CPU cpu, Variables variables)
{
	
}