using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace processLogServicesIIB
{
    class Program
    {
        static void Main(string[] args)
        {
            string origen = String.Empty;
            string destino = String.Empty;

            string lineLog;
            string lineReporte;
            string processID;
            string processIDAnterior = "";
            string fechaInicio = "";
            string fechaFin = "";
            string folderPath = System.IO.Directory.GetCurrentDirectory();

            int contadorRegistros = 0;

            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("La sintaxis del comando no es correcta.");
                    Console.WriteLine();
                    Console.WriteLine("-f    Nombre del archivo origen de log");
                    Console.WriteLine();
                    Console.WriteLine("-r    Nombre del archivo destino con el reporte del procesamiento");
                    Console.WriteLine();
                    return;
                }

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-F":
                        case "-f": // Archivo de log origen
                            if (args.Length > i + 1)
                            {
                                origen = ValidarArgumento(args[i + 1].Trim());
                            }
                            break;

                        case "-R":
                        case "-r": // Archivo con el reporte de procesamiento
                            if (args.Length > i + 1)
                            {
                                destino = ValidarArgumento(args[i + 1].Trim().ToLower());
                            }
                            break;
                    }
                }

                if (origen == String.Empty || origen == "")
                {
                    Console.WriteLine("Error: El argumento -f [archivo de log origen] está vacío");
                    Console.WriteLine();
                    return;
                }

                if (destino == String.Empty || destino == "")
                {
                    destino = System.IO.Directory.GetCurrentDirectory() + "\reporte.txt";
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }


            StreamReader fsFileLog = new StreamReader(origen, false);
            StreamWriter fsFileReporte = new StreamWriter(destino, false);

            lineLog = fsFileLog.ReadLine();
            while (lineLog != null)
            {
                string[] partLog = lineLog.Split('|');

                processID = partLog[4];

                if (lineLog.IndexOf("GUPMAPI request execute: ") > 0)
                {
                    contadorRegistros++;
                }
                               
                if (processIDAnterior == "")
                {
                    fechaInicio = partLog[0].Substring(0, 19);
                    processIDAnterior = processID;
                }

                if (processID != processIDAnterior) 
                {
                    lineReporte = "ProcessID: " + processIDAnterior + " Cantidad de Registros: " + contadorRegistros.ToString() +  " Inicio: " + fechaInicio + " Fin: " + fechaFin + " Duración: " + (DateTime.Parse(fechaFin) - DateTime.Parse(fechaInicio));
                    fechaInicio = partLog[0].Substring(0, 19);
                    processIDAnterior = processID;
                    contadorRegistros = 0;
                    fsFileReporte.WriteLine(lineReporte);
                }

                fechaFin = partLog[0].Substring(0, 19);
                lineLog = fsFileLog.ReadLine();
            }

            lineReporte = "ProcessID: " + processIDAnterior + " Cantidad de Registros: " + contadorRegistros.ToString() + " Inicio: " + fechaInicio + " Fin: " + fechaFin + " Duración: " + (DateTime.Parse(fechaFin) - DateTime.Parse(fechaInicio));
            fsFileReporte.WriteLine(lineReporte);

            fsFileReporte.Close();
            fsFileLog.Close();
        }

        public static string ValidarArgumento(string argumento)
        {
            string[] args = { "-f", "-F", "-r", "-R"};

            bool isArg = false;

            foreach (string arg in args)
            {
                if (arg == argumento)
                {
                    isArg = true;
                    break;
                }
            }
            return isArg ? String.Empty : argumento;
        }

    }
}
