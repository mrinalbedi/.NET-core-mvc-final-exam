using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Sail.Models
{
    [ModelMetadataType(typeof(MemberMetaData))]
    public partial class Member:IValidatableObject
    {
        SailContext _context = new SailContext();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (MemberId != 0)
            //{
            //    FullName = FirstName + ' ' + LastName + " & " + SpouseFirstName;
            //}
            if(ProvinceCode!=null)
            {
                if (ProvinceCode.Length != 2)
                {
                    yield return new ValidationResult("The Province Code should be exactly 2 characters long", new[] { "ProvinceCode" });
                }
                else
                {
                    var province = _context.Province.Where(m => m.ProvinceCode == ProvinceCode).FirstOrDefault();
                    if (province==null)
                    {
                        yield return new ValidationResult("The Province Code is not valid", new[] { "ProvinceCode" });
                    }
                    else
                    {
                        ProvinceCode = ProvinceCode.Trim().ToUpper();
                    }
                }     
            }
            Regex PostalCodePattern = new Regex((@"^[a-zA-Z]{1}[0-9]{1}[a-zA-Z]{1}\s{0,1}[0-9]{1}[a-zA-Z]{1}[0-9]{1}"),RegexOptions.IgnoreCase);
            if (PostalCodePattern.IsMatch(PostalCode.Trim()))
            {
                if (!PostalCode.Contains(" "))
                {
                    PostalCode = PostalCode.Insert(3, " ");
                    PostalCode = PostalCode.Trim().ToUpper();
                }
                else
                    PostalCode = PostalCode.Trim().ToUpper();   
            }
            else
            {
                yield return new ValidationResult("The Postal Code entered is not in valid canadian format", new[] { "PostalCode" });
            }

   
            Regex HomePhonePattern = new Regex(@"^\d\d\d-{0,1}\d\d\d-{0,1}\d\d\d\d");
            if (HomePhone != null)
            {
                if (HomePhonePattern.IsMatch(HomePhone))
                {
                    if (!HomePhone.Contains('-'))
                    {
                        HomePhone = HomePhone.Insert(3, "-");
                        HomePhone = HomePhone.Insert(7, "-");
                        HomePhone = HomePhone.Trim();
                    }
                }
                else
                {
                    yield return new ValidationResult("The home Phone number entered is not in valid format 999-999-9999", new[] { "HomePhone" });
                }
            }

            if(string.IsNullOrEmpty(SpouseFirstName) && string.IsNullOrEmpty(SpouseLastName))
            {
                FullName = LastName.Trim() + "," + FirstName.Trim();
            }
            else
            {
                if(SpouseLastName==null || SpouseLastName==LastName)
                {
                    FullName = FirstName.Trim() + ' ' + LastName.Trim() + " & " + SpouseFirstName.Trim();
                }
                else
                {
                    FullName = LastName.Trim() + "," + FirstName.Trim() + " & " + SpouseLastName.Trim() + "," + SpouseFirstName.Trim();
                }
            }

            if(UseCanadaPost)
            {
                if(string.IsNullOrEmpty( Street) && string.IsNullOrEmpty(City))
                {
                    yield return new ValidationResult("The Street name and City Name field are required fields if you have checked Canada Post checkbox", new[] { "Street" });
                }
            }
            else
            {
                if(string.IsNullOrEmpty(Email))
                {
                    yield return new ValidationResult("The Email address field is required", new[] { "Email" });
                }
            }
            if (MemberId == 0)
            {
                var duplicateID = _context.Member.Where(x => x.MemberId == MemberId).FirstOrDefault();
                if (duplicateID != null)
                {
                    yield return new ValidationResult("The Member Id entered is already on file", new[] { "MemberId" });
                }
            }
            if (Street!=null)
            {
                Street = Street.Trim();
            }
            if(City!=null)
            {
                City = City.Trim();
            }
            if(Email!=null)
            {
                Email = Email.Trim();
            }    
            if(Comment!=null)
            {
                Comment = Comment.Trim();
            }
            yield return ValidationResult.Success;
        }
    }
    public class MemberMetaData
    {
        public int MemberId { get; set; }

        
        public string FullName { get; set; }

        [Required(ErrorMessage = "The First Name field is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The Last Name field is required")]
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }

        [Required(ErrorMessage ="The province code field is required")]
        
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int? YearJoined { get; set; }
        public string Comment { get; set; }
        public bool TaskExempt { get; set; }
        public bool UseCanadaPost { get; set; }
    }
}
