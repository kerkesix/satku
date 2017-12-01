namespace KsxEventTracker.Registration.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegistrationViewModel
    {
        [EmailAddress(ErrorMessage = "Anna toimiva sähköpostiosoite. Osoite tarkistetaan lähettämällä siihen sähköposti.")]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Sähköpostiosoite on pakollinen. Miten me muuten saisimme sinut kiinni?")]
        [Display(Name = "Sähköpostiosoite")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kännykkänumero on pakollinen tieto. Tämä tarvitaan oman turvallisuutesi vuoksi. Anna numero, josta sinut saa kiinni kävelyn aikana.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Matkapuhelin")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Etunimi on pakollinen tieto.")]
        [Display(Name = "Etunimi")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Sukunimi on pakollinen tieto.")]
        [Display(Name = "Sukunimi")]
        public string Lastname { get; set; }

        [Display(Name = "Olen ollut aiemmin satkulla")]
        public bool BeenThere { get; set; }

        [Display(Name = "Muuta, mitä järjestäjien olisi hyvä tietää")]
        public string Info { get; set; }

        [Display(Name = "Satkuseurannassa näkyvä nimi")]
        [StringLength(40, ErrorMessage = "Nimen pitää olla vähintään 4 ja korkeintaan 40 merkkiä pitkä", MinimumLength = 4)]
        public string Nickname { get; set; }
    }
}