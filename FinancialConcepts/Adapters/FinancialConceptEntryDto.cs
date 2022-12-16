/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Interface adapters                      *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Data Transfer Object                    *
*  Type     : FinancialConceptEntryDto                   License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Data transfer object for FinancialConceptEntry instances.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Newtonsoft.Json;

using Empiria.FinancialAccounting.ExternalData.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.Adapters {

  /// <summary>Data transfer object for FinancialConceptEntry instances.</summary>
  public class FinancialConceptEntryDto : IDto {

    internal FinancialConceptEntryDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }


    public FinancialConceptEntryType Type {
      get; internal set;
    }


    public ShortFlexibleEntityDto Account {
      get; internal set;
    }


    public ShortFlexibleEntityDto SubledgerAccount {
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


    public ExternalVariableDto ExternalVariable {
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


    public Positioning Positioning {
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


    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public NamedEntityDto Group {
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
