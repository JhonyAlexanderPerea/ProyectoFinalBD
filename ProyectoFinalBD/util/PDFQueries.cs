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
        
        public DataTable GetEquiposConMasFallasPorTipo()
        {
            string query = @"
        SELECT 
            te.nombreTipoEquipo,
            COUNT(d.codigoDanio) AS totalDanios
        FROM 
            TipoEquipo te
        JOIN 
            Equipo e ON te.codigoTipoEquipo = e.tipoEquipo
        JOIN 
            Danio d ON e.codigoEquipo = d.equipo
        GROUP BY 
            te.nombreTipoEquipo
        ORDER BY 
            totalDanios DESC";

            return ExecuteQuery(query);
        }
        
        public DataTable GetCostosMantenimientoPorProveedor()
        {
            string query = @"
        SELECT 
            p.nombreProveedor,
            SUM(m.costoMantenimiento) AS totalGastoMantenimiento
        FROM 
            Proveedor p
        JOIN 
            Equipo e ON p.codigoProveedor = e.proveedor
        JOIN 
            Mantenimiento m ON e.codigoEquipo = m.equipo
        GROUP BY 
            p.nombreProveedor
        ORDER BY 
            totalGastoMantenimiento DESC";

            return ExecuteQuery(query);
        }
        
        public DataTable GetEquiposFueraDePlazo()
        {
            string query = @"
        SELECT 
            u.nombreUser,
            e.nombreEquipo,
            pr.fechaPrestamo,
            pr.fechaLimitePrestamo,
            d.fechaDevolution
        FROM 
            Prestamo pr
        JOIN 
            Devolucion d ON pr.codigoPrestamo = d.prestamo
        JOIN 
            Usuario u ON pr.usuario = u.codigoUser
        JOIN 
            Equipo e ON pr.equipo = e.codigoEquipo
        WHERE 
            d.fechaDevolution > pr.fechaLimitePrestamo";

            return ExecuteQuery(query);
        }

        public DataTable GetEquiposPorMunicipioYTipo()
        {
            string query = @"
        SELECT 
            m.nombreMunicipio,
            te.nombreTipoEquipo,
            COUNT(e.codigoEquipo) AS totalEquipos
        FROM 
            Equipo e
        JOIN 
            TipoEquipo te ON e.tipoEquipo = te.codigoTipoEquipo
        JOIN 
            Proveedor p ON e.proveedor = p.codigoProveedor
        JOIN 
            Municipio m ON p.municipio = m.codigoMunicipio
        GROUP BY 
            m.nombreMunicipio, te.nombreTipoEquipo
        ORDER BY 
            m.nombreMunicipio, totalEquipos DESC";

            return ExecuteQuery(query);
        }

        public DataTable GetAccesosPorRol()
        {
            string query = @"
        SELECT 
            ru.nombreRolUsuario,
            COUNT(lu.codigoLogUser) AS totalAccesos
        FROM 
            LogUsuario lu
        JOIN 
            Usuario u ON lu.usuario = u.codigoUser
        JOIN 
            RolUsuario ru ON u.rolUsuario = ru.codigoRolUsuario
        GROUP BY 
            ru.nombreRolUsuario
        ORDER BY 
            totalAccesos DESC";

            return ExecuteQuery(query);
        }


    }