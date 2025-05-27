using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ProyectoFinalBD.util;

public class PDFQueries
    {
        // ✔ Usa un usuario específico distinto de SYS para mayor seguridad en producción
        private static readonly string _connectionString =
            "User Id=sys;Password=admin;Data Source=localhost:1521/xe;DBA Privilege=SYSDBA";

        private static DataTable ExecuteQuery(string query)
        {
            using (var conn = new OracleConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new OracleCommand(query, conn))
                using (var adapter = new OracleDataAdapter(cmd))
                {
                    var table = new DataTable();
                    adapter.Fill(table);
                    return table;
                }
            }
        }

        // 1. Equipos más prestados
        public  DataTable GetEquiposMasPrestados()
        {
            string query = @"
                SELECT E.NOMBREEQUIPO AS NOMBRE_DEL_EQUIPO, COUNT(P.CODIGOPRESTAMO) AS CANTIDAD_PRESTAMOS
                FROM EQUIPO E
                JOIN PRESTAMO P ON E.CODIGOEQUIPO = P.EQUIPO
                GROUP BY E.NOMBREEQUIPO
                ORDER BY CANTIDAD_PRESTAMOS DESC";
            return ExecuteQuery(query);
        }

        // 2. Usuarios con más préstamos
        public  DataTable GetUsuariosConMasPrestamos()
        {
            string query = @"
                SELECT U.NOMBREUSER, COUNT(P.CODIGOPRESTAMO) AS CANTIDAD_PRESTAMOS
                FROM USUARIO U
                JOIN PRESTAMO P ON U.CODIGOUSER = P.USUARIO
                GROUP BY U.NOMBREUSER
                ORDER BY CANTIDAD_PRESTAMOS DESC";
            return ExecuteQuery(query);
        }

        // 3. Reporte de equipos en reparación
        public  DataTable GetEquiposEnReparacion()
        {
            string query = @"
                SELECT E.NOMBREEQUIPO, M.FECHAMANTENIMIENTO, M.HALLAZGOSMANTENIMIENTO, M.COSTOMANTENIMIENTO
                FROM EQUIPO E
                JOIN MANTENIMIENTO M ON E.CODIGOEQUIPO = M.EQUIPO
                WHERE E.ESTADOEQUIPO = 'EST003'";
            return ExecuteQuery(query);
        }

        // 4. Préstamos activos por tipo de equipo
        public  DataTable GetPrestamosActivosPorTipoEquipo()
        {
            string query = @"
                SELECT T.NOMBRETIPOEQUIPO AS TIPO_DE_EQUIPO, COUNT(P.CODIGOPRESTAMO) AS PRESTAMOS_ACTIVOS
                FROM TIPOEQUIPO T
                JOIN EQUIPO E ON T.CODIGOTIPOEQUIPO = E.TIPOEQUIPO
                JOIN PRESTAMO P ON E.CODIGOEQUIPO = P.EQUIPO
                WHERE E.ESTADOEQUIPO = 'EST002'
                GROUP BY T.NOMBRETIPOEQUIPO";
            return ExecuteQuery(query);
        }

        // 5. Estadísticas de mantenimiento de equipos
        public  DataTable GetEstadisticasMantenimiento()
        {
            string query = @"
                SELECT 
                    E.NOMBREEQUIPO,
                    COUNT(M.CODIGOMANTENIMIENTO) AS TOTAL_MANTENIMIENTOS,
                    AVG(M.COSTOMANTENIMIENTO) AS PROMEDIO_COSTO_MANTENIMIENTO,
                    MAX(M.FECHAMANTENIMIENTO) AS ULTIMO_MANTENIMIENTO
                FROM EQUIPO E
                JOIN MANTENIMIENTO M ON E.CODIGOEQUIPO = M.EQUIPO
                GROUP BY E.NOMBREEQUIPO";
            return ExecuteQuery(query);
        }
    }