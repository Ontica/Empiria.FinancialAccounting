/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Information Holder                      *
*  Type     : TrialBalanceValued                         License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Contains entries from TrialBalanceValued.                                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.BalanceEngine.Data;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Contains entries from TrialBalanceValued.</summary>
  public class TrialBalanceValued {

    #region Constructors and parsers

    protected TrialBalanceValued() {
      // Require by Empiria FrameWork
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_CUENTA_ESTANDAR", ConvertFrom = typeof(long))]
    public int IdCuentaEstandar {
      get; set;
    } = 0;

    [DataField("FECHA_AFECTACION", ConvertFrom = typeof(DateTime))]
    public DateTime FechaAfectacion {
      get; set;
    } = ExecutionServer.DateMinValue;


    [DataField("NATURALEZA", Default = DebtorCreditorType.Deudora)]
    public DebtorCreditorType Naturaleza {
      get; private set;
    } = DebtorCreditorType.Deudora;

    [DataField("ID_CUENTA_AUXILIAR", ConvertFrom = typeof(long))]
    public long IdCuentaAuxiliar {
      get; private set;
    }

    [DataField("ID_SECTOR", ConvertFrom = typeof(long))]
    public Sector Sector {
      get; private set;
    }

    [DataField("ID_MONEDA", ConvertFrom = typeof(decimal))]
    public Currency Currency {
      get; private set;
    }

    [DataField("DEBE", ConvertFrom = typeof(decimal))]
    public decimal Debe {
      get; private set;
    }

    [DataField("HABER", ConvertFrom = typeof(decimal))]
    public decimal Haber {
      get; private set;
    }

    [DataField("EXCHANGE_RATE", ConvertFrom = typeof(decimal))]
    public decimal TipoCambio {
      get; private set;
    }

    [DataField("DEBE_VAL", ConvertFrom = typeof(decimal))]
    public decimal DebeValorizado {
      get; private set;
    }

    [DataField("HABER_VAL", ConvertFrom = typeof(decimal))]
    public decimal HaberValorizado {
      get; private set;
    }

    #endregion Properties

    #region Public methods

    static public FixedList<TrialBalanceValued> GetTrialBalanceValuedTransactions(DateTime fromDate, DateTime toDate, int ExchangeRateTypeId) {

      return TrialBalanceValuedDataService.GetTransactions(fromDate, toDate, ExchangeRateTypeId);
    }


    #endregion Public methods


  } // class TrialBalanceValued

} // namespace Empiria.FinancialAccounting.BalanceEngine
