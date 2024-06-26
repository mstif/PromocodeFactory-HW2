﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Exeptions;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NoContent();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }
        /// <summary>
        /// Создать новый элемент
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(Employee employee)
        {
            if (employee != null)
            {
                try
                {
                    var result = await _employeeRepository.Create(employee);

                    return Ok(result);
                }
                catch(DoubleElementException dex)
                {
                    return StatusCode(409,dex.Message);
                }
                catch (Exception ex)
                {
                    return StatusCode(520);
                }
            }
            else return BadRequest();




        }
        /// <summary>
        /// Обновить элемент
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateEmployeeAsync(Employee employee)
        {
            if (employee != null)
            {
                try
                {
                    await _employeeRepository.Update(employee);
                    return Ok(employee);
                }
                catch(ElementNotExistsExeption nex)
                {
                    return BadRequest(nex.Message);
                }
                catch ( Exception ex)
                {
                    return StatusCode(520);
                }
            }
            else
            {
                return BadRequest();
            }

            


        }
        /// <summary>
        /// Удалить элемент по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeAsync(Guid id)
        {
            try
            {
                await _employeeRepository.Delete(id);
                return Ok();
            }
            catch(ElementNotExistsExeption nex)
            {
                return BadRequest(nex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(520);
            }




        }

    }
}