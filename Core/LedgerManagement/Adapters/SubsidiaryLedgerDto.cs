/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Ledger Management                          Component : Interface adapters                      *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Transfer Object                    *
*  Type     : SubsidiaryLedgerDto                        License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Output DTO for subsidiary ledger books.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.Adapters {

  /// <summary>Output DTO for subsidiary ledger books.</summary>
  public class SubsidiaryLedgerDto {

    internal SubsidiaryLedgerDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public string TypeName {
      get; internal set;
    }


    public NamedEntityDto BaseLedger {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }


    public string Description {
      get; internal set;
    }


    public string AccountsPrefix {
      get; internal set;
    }

  }  // public class SubsidiaryLedgerDto


  /// <summary>Output DTO for subledger accounts.</summary>
  public class SubledgerAccountDescriptorDto {

    internal SubledgerAccountDescriptorDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }

    public string Number {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string FullName {
      get; internal set;
    }

  }  // class SubledgerAccountDto


  /// <summary>Output DTO for a subsidary ledger account.</summary>
  public class SubsidiaryAccountDto {

    internal SubsidiaryAccountDto() {
      // no-op
    }

    public int Id {
      get; internal set;
    }


    public NamedEntityDto Ledger {
      get; internal set;
    }

    public NamedEntityDto SubsidiaryLedger {
      get; internal set;
    }


    public string Number {
      get; internal set;
    }


    public string Name {
      get; internal set;
    }

    public string Keywords {
      get; internal set;
    }

    public string Description {
      get; internal set;
    }

  }  // class SubsidiaryAccountDto

}  // namespace Empiria.FinancialAccounting.Adapters
