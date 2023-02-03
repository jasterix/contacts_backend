using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using contactsApi.Models;
using contactsApi.Services;

namespace contactsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ContactsController : ControllerBase
    {
        private readonly ILogger<ContactsController> _logger;
        private readonly ContactContext _context;

        public ContactsController(ContactContext context, ILogger<ContactsController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Get all
        // GET: api/Contacts
        [HttpGet(Name = "GetAll")]
        public async Task<ActionResult<List<Contact>>> GetContacts()
        {
            var total = _context.Contacts.ToListAsync().Result.Count;
            if (!(total > 0))
            {
                // Seed data
                return ContactService.GetAll();
            }
            return await _context.Contacts.ToListAsync();
        }

        // CREATE
        // POST: api/Contacts
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost(Name = "Create")]
        public async Task<ActionResult<Contact>> PostContact(Contact contact)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ContactContext.Contacts'  is null.");
            }
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contact);
        }

        // Read
        // GET: api/Contacts/5
        [HttpGet("{id}", Name = "Read")]
        public async Task<ActionResult<Contact>> GetContact(long id)
        {
            if (_context.Contacts == null)
            {
                return NotFound();
            }
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return contact;
        }

        // Update
        // PUT: api/Contacts/5
        // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}", Name = "Update")]
        public async Task<IActionResult> PutContact(long id, Contact contact)
        {
            if (id != contact.Id)
            {
                return BadRequest();
            }

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // DELETE: api/Contacts/5
        [HttpDelete("{id}", Name = "Delete")]
        public async Task<IActionResult> DeleteContact(long id)
        {
            if (_context.Contacts == null)
            {
                return NotFound();
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Content("User {id} has been deleted");
        }

        private bool ContactExists(long id)
        {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}


// Alternate way of formatting actions 
// [HttpPost]
// public IActionResult CreateContact(CreateContactRequest request)
// {
//     var contact = new Contact(
//         Guid.NewGuid(),
//         request.Created,
//         request.Name
//     );

// var response = new ContactResponse(
//     contactsApi.Id,
//     contactsApi.name
// );

//     return CreatedAtAction(
//     ActionNameAttribute: nameOf(GetContact),
//     routeValues: new { id= contactsApi.Id},
//     value: response
// );
// }