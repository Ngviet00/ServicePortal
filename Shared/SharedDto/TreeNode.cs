namespace ServicePortals.Application.Dtos.User.Requests
{
    public class TreeNode
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public int? ViTriToChucId { get; set; } //vị trí của người đó
        public string? PositionName { get; set; } //vị trí của người đó
        public int? ParentPositionId { get; set; } // vị trí cấp trên của người đó
        public string? TeamName { get; set; }
        public List<TreeNode> Children { get; set; } = [];
    }
}