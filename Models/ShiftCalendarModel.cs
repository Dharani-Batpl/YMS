namespace YardManagementApplication.Models
{
    public class ShiftCalendarModel
    {
        public int AssignmentId { get; set; }

        public int PlantId { get; set; }

        public int TemplateId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Sun { get; set; }

        public bool Mon { get; set; }

        public bool Tue { get; set; }

        public bool Wed { get; set; }

        public bool Thu { get; set; }
        public bool Fri { get; set; }

        public bool Sat { get; set; }

        public bool IsActive { get; set; }
    }

    public class ShiftCalendarViewModel
    {
        public List<YardManagementApplication.TemplateModel> Templates { get; set; }
        public List<YardManagementApplication.PlantMasterModel> Plants { get; set; }
    }

}
