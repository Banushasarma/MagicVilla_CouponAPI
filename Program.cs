using MagicVilla_CouponAPI.Data;
using MagicVilla_CouponAPI.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Get Coupon call 
app.MapGet("/api/coupon", () =>
{
    return Results.Ok(CouponStore.Coupons);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);

//Get Coupon by Id call
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.Coupons.FirstOrDefault(c => c.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(200);

//Add Coupon call
app.MapPost("/api/coupon", ([FromBody] Coupon coupon) =>
{
    if (coupon.Id != 0 || string.IsNullOrEmpty(coupon.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name.");
    }
    if (CouponStore.Coupons.FirstOrDefault(u => u.Name == coupon.Name).ToString() == coupon.Name)
    {
        return Results.BadRequest("Coupon name already exists.");
    }

    coupon.Id = CouponStore.Coupons.Max(c => c.Id) + 1;
    CouponStore.Coupons.Add(coupon);

    //return Results.Created("GetCoupon", coupon);
    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, coupon);
}).WithName("CreateCoupon").Accepts<Coupon>("application/json").Produces<Coupon>(201).Produces(400);

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
