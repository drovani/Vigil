﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Vigil.Validation
{
    public abstract class ValidationRule<TModel> : IRule<TModel>
    {
        public Guid RuleId { get; protected set; }
        public int Ordinal { get; protected set; }
        public string ErrorToken { get; protected set; }
        public string[] ParticipatingFields { get; protected set; }

        public ValidationRule(Guid validationRuleId = default(Guid), int ordinal = 0, string errorToken = "Error", params string[] participatingFields)
        {
            RuleId = validationRuleId;
            Ordinal = ordinal;
            ErrorToken = errorToken;
            ParticipatingFields = participatingFields ?? new string[0];
        }

        public abstract ValidationResult Validate(TModel target);
    }
}