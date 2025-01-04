using AutoMapper;
using MagicVilla_CouponAPI;
using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using MagicVilla_CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Get Coupon call 
app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "GetCoupons called");
    return Results.Ok(CouponStore.Coupons);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

//Get Coupon by Id call
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(200);

//Add Coupon call
app.MapPost("/api/coupon", (IMapper _mapper, [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    if (string.IsNullOrEmpty(coupon_C_DTO.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name.");
    }
    if (CouponStore.Coupons.FirstOrDefault(u => u.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        return Results.BadRequest("Coupon name already exists.");
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    coupon.Id = CouponStore.Coupons.Max(c => c.Id) + 1;
    CouponStore.Coupons.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    //return Results.Created("GetCoupon", coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<Coupon>(201).Produces(400);

//Update Coupon call
app.MapPut("/api/coupon/{id:int}", (int id, [FromBody] Coupon coupon) =>
{
    var existingCoupon = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        return Results.NotFound();
    }
    existingCoupon.Name = coupon.Name;
    existingCoupon.Percent = coupon.Percent;
    existingCoupon.IsActive = coupon.IsActive;
    existingCoupon.LastUpdated = DateTime.Now;
    return Results.Ok(existingCoupon);
}).WithName("UpdateCoupon").Produces<Coupon>(200).Produces(404);

//Delete Coupon call
app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    var existingCoupon = CouponStore.Coupons.FirstOrDefault(c => c.Id == id);
    if (existingCoupon == null)
    {
        return Results.NotFound();
    }
    CouponStore.Coupons.Remove(existingCoupon);
    return Results.Ok(existingCoupon);
}).WithName("DeleteCoupon").Produces<Coupon>(200).Produces(404);


app.UseHttpsRedirection();

app.Run();
