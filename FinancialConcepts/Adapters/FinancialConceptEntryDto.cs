﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptEntryDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for FinancialConceptEntry instances.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for FinancialConceptEntry instances.</summary>
  public class FinancialConceptEntryDto {

    internal FinancialConceptEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public FinancialConceptEntryType Type {
      get; internal set;
    }


    public string AccountNumber {
      get; internal set;
    }


    public string SubledgerAccountNumber {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public string CurrencyCode {
      get; internal set;
    }


    public FinancialConceptDto ReferencedFinancialConcept {
      get; internal set;
    }


    public string ExternalVariableCode {
      get; internal set;
    }


    public string CalculationRule {
      get; internal set;
    }


    public string DataColumn {
      get; internal set;
    }


    public OperatorType Operator {
      get; internal set;
    }


    public ItemPositioning Positioning {
      get; internal set;
    }


  }  // class FinancialConceptFullEntry



  /// <summary>Short DTO for display FinancialConceptEntry instances in lists.</summary>
  public class FinancialConceptEntryDescriptorDto : IDto {

    internal FinancialConceptEntryDescriptorDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public FinancialConceptEntryType Type {
      get; internal set;
    }


    public string ItemName {
      get; internal set;
    }


    public string ItemCode {
      get; internal set;
    }


    public string SubledgerAccount {
      get; internal set;
    }


    public string SectorCode {
      get; internal set;
    }


    public string Operator {
      get; internal set;
    }

  }  // class FinancialConceptEntryDescriptorDto

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters
