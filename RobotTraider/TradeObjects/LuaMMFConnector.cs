using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeObjects.LuaConnection
{
    public class LuaMMFConnector
    {
        public string Label { get; set; }
        public MemoryMappedFile MemMapFile;
        StreamReader SR_Terminal;
        StreamWriter SW_Terminal;
        public LuaMMFConnector(string label)
        {
            this.Label = label;
            MemMapFile = MemoryMappedFile.CreateOrOpen( label, 1400, MemoryMappedFileAccess.ReadWrite);
            SR_Terminal = new StreamReader(MemMapFile.CreateViewStream(), System.Text.Encoding.Default);
            SW_Terminal = new StreamWriter(MemMapFile.CreateViewStream(), System.Text.Encoding.Default);
        }

        public string GetData()
        {
            var Strdate = GetTerminalQuoteData();
            if (Strdate != "00" && Strdate != "" && Strdate != "0" && Strdate != "-1")
            {
                SetTerminalQuoteData();
            }
            return Strdate;
        }
        // Читает данные из StreamReader
        private string GetTerminalQuoteData()
        {
            SR_Terminal.BaseStream.Seek(0, SeekOrigin.Begin);
            return SR_Terminal.ReadToEnd().Trim('\0', '\r', '\n');
        }
        // Очищает память, сообщая тем самым терминалу, что данные получены
        private void SetTerminalQuoteData()
        {
            SW_Terminal.BaseStream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 1400; i++)
                SW_Terminal.Write("\0");
            SW_Terminal.Flush();
        }


        public void SendData(string data)
        {
            SetQUIKCommandData(data);
        }
        //Функция отправки команды в QUIK ( вызов: SetQUIKCommandData("Ваша команда"); ), 
        //или очистки памяти, при вызове без параметров ( вызов: SetQUIKCommandData(); )
        private void SetQUIKCommandData(string Data = "")
        {
            //Если нужно отправить команду
            if (Data != "" && Data != null)
            {
                //Дополняет строку команды "нулевыми байтами" до нужной длины
                for (int i = Data.Length; i < 256; i++) Data += "\0";
            }
            else //Если нужно очистить память
            {
                //Заполняет строку для записи "нулевыми байтами"
                for (int i = 0; i < 256; i++) Data += "\0";
            }
            //Встает в начало
            SW_Terminal.BaseStream.Seek(0, SeekOrigin.Begin);
            //Записывает строку
            SW_Terminal.Write(Data);
            //Сохраняет изменения в памяти
            SW_Terminal.Flush();
        }

    }
}
