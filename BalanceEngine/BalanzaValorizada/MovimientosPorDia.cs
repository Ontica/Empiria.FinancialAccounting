/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : MovimientosPorDia                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contiene la suma de cargos y abonos por fecha y cuenta.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Data;
using Empiria.Time;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contiene la suma de cargos y abonos por fecha y cuenta.</summary>
  public class MovimientosPorDia {

    #region Constructors and parsers

    static internal FixedList<MovimientosPorDia> GetList(TimePeriod period) {

      var sql =
        "SELECT id_cuenta_estandar, id_moneda, fecha_afectacion, " +
               "SUM(debe) Cargos, SUM(haber) Abonos " +
        "FROM vw_cof_movimiento " +
        $"WHERE " +
            $"fecha_afectacion >= {DataCommonMethods.FormatSqlDbDate(period.StartTime)} AND " +
            $"fecha_afectacion < {DataCommonMethods.FormatSqlDbDate(period.EndTime.AddDays(1))}" +
        "GROUP BY " +
            "id_cuenta_estandar, id_moneda, fecha_afectacion";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<MovimientosPorDia>(op);
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    internal StandardAccount CuentaEstandar {
      get; private set;
    }


    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    internal Currency Moneda {
      get; private set;
    }


    [DataField("FECHA_AFECTACION", ConvertFrom = typeof(DateTime))]
    public DateTime FechaAfectacion {
      get; private set;
    }


    [DataField("CARGOS", ConvertFrom = typeof(decimal))]
    public decimal Cargos {
      get; private set;
    }


    [DataField("ABONOS", ConvertFrom = typeof(decimal))]
    public decimal Abonos {
      get; private set;
    }

    #endregion Properties

  } // class MovimientosPorDia

} // namespace Empiria.FinancialAccounting.BalanceEngine
