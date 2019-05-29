using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDevCenter.MainFrame
{
    public sealed class ConfigurationEmulator
    {
        #region ::. Propriedades .::

        /// <summary>
        /// Host para acesar o Mainfame
        /// </summary>
        public string Host { get; }
       
        /// <summary>
        /// Porta para acessar ao Mainfame
        /// </summary>
        public int Port { get; }
        
        /// <summary>
        /// Usuário de Acesso 
        /// </summary>
        public string User { get; private set; }
        
        /// <summary>
        /// Senha para Acesso 
        /// </summary>
        public string Password { get; private set; }
       
        /// <summary>
        /// Indica se é para usar Certificado SSL
        /// </summary>
        public bool UseSsl { get; }
        
        /// <summary>
        /// Informa o tipo de Mainframe
        /// </summary>
        public string Tipo { get; }
        
        /// <summary>
        /// Tempo de timeout
        /// </summary>
        public int TimeOut { get; }

        /// <summary>
        /// OffiSet da coluna
        /// </summary>
        public int OffSetColumn { get; }

        /// <summary>
        /// OffiSet da linha
        /// </summary>
        public int OffSetRow { get; }

        #endregion


        /// <summary>
        /// Construtor Padrão
        /// </summary>
        /// <param name="host">Host para Acesso ao Mainfame</param>
        /// <param name="port">Porta de Acesso ao Mainfame</param> 
        /// <param name="useSsl">Indica se é para usar Certificado SSL</param>
        /// <param name="tipo">Informa o tipo de Mainframe</param>
        /// <param name="timeOut">Tempo de timeout</param>
        /// <param name="offSetColumn">Considerar menos para coluna</param>
        /// <param name="offSetRow">Considerar menos para linha</param>
        public ConfigurationEmulator(string host, int port, bool useSsl, string tipo, int timeOut, int offSetColumn, int offSetRow)
        {
            Host = host;
            Port = port;
            UseSsl = useSsl;
            Tipo = tipo;
            TimeOut = timeOut;
            OffSetColumn = offSetColumn;
            OffSetRow = offSetRow;
        }

        /// <summary>Seta usuário e senha para login no mainframe</summary>
        /// <param name="user">Usuário de Acesso ao Mainframe</param>
        /// <param name="password">Senha para Acesso ao Mainfame</param>
        public void Setcredentials(string user, string password)
        {
            User = user;
            Password = password;
        }
    }
}
