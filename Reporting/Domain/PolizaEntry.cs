/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reporting Services                         Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reporting.dll          Pattern   : Empiria Plain Object                    *
*  Type     : PolizaEntry                                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Represents an entry for a policy.                                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using Empiria.FinancialAccounting.Vouchers;

namespace Empiria.FinancialAccounting.Reporting.Domain {

  public interface IPolizaEntry {
  
  }

  /// <summary>Represents an entry for a policy.</summary>
  public class PolizaEntry : IPolizaEntry {


    #region Constructors and parsers

    internal PolizaEntry() {
      // Required by Empiria Framework.
    }

    #endregion Constructors and parsers

    [DataField("ID_MAYOR", ConvertFrom = typeof(decimal))]
    public Ledger Ledger {
      get;
      internal set;
    }

    [DataField("ID_TRANSACCION", ConvertFrom = typeof(decimal))]
    public Voucher Voucher {
      get;
      internal set;
    }

    [DataField("DEBE")]
    public decimal Debit {
      get;
      internal set;
    }


    [DataField("HABER")]
    public decimal Credit {
      get;
      internal set;
    }

  } // class PolizaEntry

} // namespace Empiria.FinancialAccounting.Reporting.Domain
