/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Empiria Data Object                     *
*  Type     : TransactionalSystemRule                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds rule data for a transactional system.                                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Holds rule data for transactional system.</summary>
  public class TransactionalSystemRule {

    #region Properties

    [DataField("ID_MAPEO_SISTEMA_POLIZA")]
    public int Id {
      get; private set;
    }


    [DataField("ID_SISTEMA_ORIGEN")]
    public int SourceSystemId {
      get; private set;
    }


    [DataField("ID_TIPO_POLIZA_ORIGEN")]
    public int SourceVoucherTypeId {
      get; private set;
    }


    [DataField("ID_TIPO_POLIZA")]
    public VoucherType TargetVoucherType {
      get; private set;
    }


    [DataField("ID_TIPO_TRANSACCION")]
    public TransactionType TargetTransactionType {
      get; private set;
    }


    [DataField("CONFIGURACION")]
    public string Configuration {
      get; private set;
    }

    #endregion Properties

  } // class TransactionalSystemRule

}  // namespace Empiria.FinancialAccounting.Vouchers
